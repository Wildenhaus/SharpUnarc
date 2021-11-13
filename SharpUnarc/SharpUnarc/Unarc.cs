using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SharpUnarc.Events;

namespace SharpUnarc
{

  /// <summary>
  ///   A wrapper for interacting with Unarc.dll.
  /// </summary>
  public partial class Unarc
  {

    #region Delegates

    /// <summary>
    ///   A delegate for handling Unarc callbacks.
    /// </summary>
    private delegate int UnarcCallbackHandler( string what, int int1, int int2, string str );

    /// <summary>
    ///   A delegate for listening to Unarc events.
    /// </summary>
    /// <param name="unarc">
    ///   The <see cref="Unarc" /> intance.
    /// </param>
    /// <param name="eventData">
    ///   The event data.
    /// </param>
    public delegate void UnarcEventHandler<T>( Unarc unarc, T eventData )
      where T : EventData;

    /// <summary>
    ///   Handles a file overwrite request.
    /// </summary>
    /// <param name="eventData">
    ///   The <see cref="OverwriteFileRequestEventData" />.
    /// </param>
    /// <returns>
    ///   An <see cref="OverwriteFileRequestResponse" /> specifying how to proceed.
    /// </returns>
    public delegate OverwriteFileRequestResponse HandleFileOverwriteRequestDelegate( OverwriteFileRequestEventData eventData );

    /// <summary>
    ///   Handles an archive password request.
    /// </summary>
    /// <param name="eventData">
    ///   The <see cref="PasswordRequestEventData" />.
    /// </param>
    /// <param name="password">
    ///   The password of the archive.
    /// </param>
    /// <returns>
    ///   The <see cref="PasswordRequestResponse" />.
    /// </returns>
    public delegate PasswordRequestResponse HandlePasswordRequestDelegate( PasswordRequestEventData eventData, out string password );

    #endregion

    #region Events

    /// <summary>
    ///   An event that occurs when any Unarc event message is received.
    /// </summary>
    public event UnarcEventHandler<EventData> EventReceived;

    /// <summary>
    ///   An event that occurs at the start of unpacking and provides the total archive size.
    /// </summary>
    public event UnarcEventHandler<ArchiveSizeEventData> ArchiveSizeEventReceived;

    /// <summary>
    ///   An event that occurs when bytes have been read from the archive.
    /// </summary>
    public event UnarcEventHandler<BytesReadEventData> BytesReadEventReceived;

    /// <summary>
    ///   An event that occurs when bytes have been written to extracted files.
    /// </summary>
    public event UnarcEventHandler<BytesWrittenEventData> BytesWrittenEventReceived;

    /// <summary>
    ///   An event that occurs when a file is about to be extracted and provides information
    ///   about the file.
    /// </summary>
    public event UnarcEventHandler<FileInfoEventData> FileInfoEventReceived;

    /// <summary>
    ///   An event that occurs when an error is encountered.
    /// </summary>
    public event UnarcEventHandler<ErrorEventData> ErrorEventReceived;

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total number
    ///   of files inside the archive.
    /// </summary>
    public event UnarcEventHandler<ArchiveTotalFilesEventData> ArchiveTotalFilesEventReceived;

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total 
    ///   uncompressed size of the archive.
    /// </summary>
    public event UnarcEventHandler<ArchiveUncompressedSizeEventData> ArchiveUncompressedSizeEventReceived;

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total 
    ///   uncompressed size of the archive.
    /// </summary>
    public event UnarcEventHandler<ArchiveCompressedSizeEventData> ArchiveCompressedSizeEventReceived;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets the delegate responsible for handling file overwrites.
    /// </summary>
    public HandleFileOverwriteRequestDelegate OverwriteFileRequestHandler { get; set; }

    /// <summary>
    ///   Gets or sets the delegate responsible for handling archive password requests.
    /// </summary>
    public HandlePasswordRequestDelegate PasswordRequestHandler { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Executes an <see cref="UnarcCommand" /> synchronously.
    /// </summary>
    /// <param name="command">
    ///   The <see cref="UnarcCommand" /> to execute.
    /// </param>
    /// <returns>
    ///   The executed command's error code.
    /// </returns>
    public UnarcExitCode ExecuteCommand( UnarcCommand command )
    {
      var argList = command.GetArgumentsList();
      if ( Debugger.IsAttached )
      {
        var argStr = string.Join( " ", argList );
        Debug.WriteLine( "Executing command: {0}", argStr );
      }

      try
      {
        var callbackHandler = new UnarcCallbackHandler( HandleUnarcCallback );
        return DynamicCallFreeArcExtract( callbackHandler, argList );
      }
      catch ( Exception exception )
      {
        Debug.WriteLine( "Error executing command: {0}", exception.Message );
        throw;
      }
    }

    /// <summary>
    ///   Executes an <see cref="UnarcCommand" /> asynchronously.
    /// </summary>
    /// <param name="command">
    ///   The <see cref="UnarcCommand" /> to execute.
    /// </param>
    /// <returns>
    ///   The executed command's error code.
    /// </returns>
    public Task<UnarcExitCode> ExecuteCommandAsync( UnarcCommand command )
    {
      var completion = new TaskCompletionSource<UnarcExitCode>();

      Task.Factory.StartNew( () =>
      {
        try
        {
          var result = ExecuteCommand( command );
          completion.TrySetResult( result );
        }
        catch ( Exception exception )
        {
          completion.TrySetException( exception );
        }
      }, TaskCreationOptions.LongRunning );

      return completion.Task;
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///   Handles an event by dispatching a message or providing data to Unarc.
    /// </summary>
    private int HandleUnarcCallback( string what, int int1, int int2, string str )
    {
      var eventMessage = new EventMessage( what, int1, int2, str );
      var eventData = EventData.CreateFromMessage( eventMessage );

      EventReceived?.Invoke( this, eventData );
      switch ( eventData.EventType )
      {
        case EventType.ArchiveSize:
          ArchiveSizeEventReceived?.Invoke( this, eventData as ArchiveSizeEventData );
          return 0;
        case EventType.BytesRead:
          BytesReadEventReceived?.Invoke( this, eventData as BytesReadEventData );
          return 0;
        case EventType.BytesWritten:
          BytesWrittenEventReceived?.Invoke( this, eventData as BytesWrittenEventData );
          return 0;
        case EventType.FileInfo:
          FileInfoEventReceived?.Invoke( this, eventData as FileInfoEventData );
          return 0;
        case EventType.Error:
          ErrorEventReceived?.Invoke( this, eventData as ErrorEventData );
          return 0;
        case EventType.ArchiveTotalFiles:
          ArchiveTotalFilesEventReceived?.Invoke( this, eventData as ArchiveTotalFilesEventData );
          return 0;
        case EventType.ArchiveUncompressedSize:
          ArchiveUncompressedSizeEventReceived?.Invoke( this, eventData as ArchiveUncompressedSizeEventData );
          return 0;
        case EventType.ArchiveCompressedSize:
          ArchiveCompressedSizeEventReceived?.Invoke( this, eventData as ArchiveCompressedSizeEventData );
          return 0;

        case EventType.OverwriteFileRequest:
          return HandleOverwriteFileRequest( eventData as OverwriteFileRequestEventData );
        case EventType.PasswordRequest:
          return HandlePasswordRequest( eventData as PasswordRequestEventData, ref str );

        default:
          Debug.WriteLine( $"Unknown Event Type: {what} | {int1} | {int2} | {str}" );
          return 1;
      }
    }

    /// <summary>
    ///   Handles an overwrite request if a handler is set, otherwise aborts extraction.
    /// </summary>
    /// <param name="eventData">
    ///   The event data.
    /// </param>
    private int HandleOverwriteFileRequest( OverwriteFileRequestEventData eventData )
    {
      var handler = OverwriteFileRequestHandler;
      if ( handler != null )
        return ( int ) handler( eventData );

      Debug.WriteLine( "File overwrite requested, but no delegate is set. Aborting." );
      return ( int ) OverwriteFileRequestResponse.Abort;
    }

    /// <summary>
    ///   Handles an archive password request if a handler is set, otherwise aborts extraction.
    /// </summary>
    /// <param name="passwordBufferSize">
    ///   The provided password buffer size.
    /// </param>
    /// <param name="passwordBuffer">
    ///   The password buffer.
    /// </param>
    private int HandlePasswordRequest( PasswordRequestEventData eventData, ref string passwordBuffer )
    {
      var handler = PasswordRequestHandler;
      if ( handler != null )
      {
        GetPassword:
        var response = handler( eventData, out var password );
        if ( string.IsNullOrEmpty( password ) )
        {
          Debug.WriteLine( "Null or empty password provided by the handler." );
          return ( int ) response;
        }

        if ( password.Length > eventData.PasswordBufferSize )
        {
          Debug.WriteLine( "A password has been provided by the handler, but it does not fit in the buffer." );
          goto GetPassword;
        }

        passwordBuffer = password;
        return ( int ) response;
      }

      Debug.WriteLine( "Password requested, but no delegate is set. Aborting." );
      return ( int ) PasswordRequestResponse.Abort;
    }

    #endregion

  }

}
