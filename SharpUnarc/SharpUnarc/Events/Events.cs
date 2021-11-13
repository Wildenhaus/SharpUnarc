using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpUnarc.Events
{

  /// <summary>
  ///   A base class for Unarc event data.
  /// </summary>
  public class EventData
  {

    #region Data Members

    /// <summary>
    ///   The original event message sent from Unarc.
    /// </summary>
    protected readonly EventMessage _message;

    #endregion

    #region Properties

    /// <summary>
    ///   The event type.
    /// </summary>
    public EventType EventType { get; }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs an <see cref="EventData" /> from an event message.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    private EventData( EventMessage message )
      : this( EventType.Unknown, message )
    {
    }

    /// <summary>
    ///   Constructs an <see cref="EventData" /> from an event message.
    /// </summary>
    /// <param name="eventType">
    ///   The type of event.
    /// </param>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    protected EventData( EventType eventType, EventMessage message )
    {
      EventType = eventType;
      _message = message;
    }

    /// <summary>
    ///   Creates a properly typed <see cref="EventData" /> from an event message.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    /// <returns>
    ///   A strongly typed <see cref="EventData" />.
    /// </returns>
    public static EventData CreateFromMessage( EventMessage message )
    {
      switch ( message.what )
      {
        case "total":
          return new ArchiveSizeEventData( message );
        case "read":
          return new BytesReadEventData( message );
        case "write":
          return new BytesWrittenEventData( message );
        case "filename":
          return new FileInfoEventData( message );
        case "overwrite?":
          return new OverwriteFileRequestEventData( message );
        case "password?":
          return new PasswordRequestEventData( message );
        case "error":
          return new ErrorEventData( message );
        case "total_files":
          return new ArchiveTotalFilesEventData( message );
        case "origsize":
          return new ArchiveUncompressedSizeEventData( message );
        case "compsize":
          return new ArchiveCompressedSizeEventData( message );
        default:
          Debug.WriteLine( $"WARNING! Unknown event type received: {message.what}" );
          return new EventData( message );
      }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///   Merges the two 32-bit int values into one 64-bit value via shifting.
    /// </summary>
    /// <returns>
    ///   The 64-bit value.
    /// </returns>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected long MergeIntValuesToLong()
    {
      uint upper = unchecked(( uint ) _message.int1);
      uint lower = unchecked(( uint ) _message.int2);
      return ( ( long ) upper << 20 ) | lower;
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "total" event.
  ///   This event occurs at the start of unpacking and provides the total archive size.
  /// </summary>
  public sealed class ArchiveSizeEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The archive's total size in bytes.
    /// </summary>
    public long ArchiveSizeInBytes
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs an <see cref="ArchiveSizeEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal ArchiveSizeEventData( EventMessage message )
      : base( EventType.ArchiveSize, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "read" event.
  ///   This event occurs when bytes have been read from the archive.
  /// </summary>
  public sealed class BytesReadEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The total number of bytes read during the extraction process.
    /// </summary>
    public long BytesRead
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="BytesReadEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal BytesReadEventData( EventMessage message )
      : base( EventType.BytesRead, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "write" event.
  ///   This event occurs when bytes have been written to extracted files.
  /// </summary>
  public sealed class BytesWrittenEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The total number of bytes written during the extraction process.
    /// </summary>
    public long BytesWritten
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="BytesWrittenEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal BytesWrittenEventData( EventMessage message )
      : base( EventType.BytesWritten, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "filename" event.
  ///   This event occurs when a file is about to be extracted and provides information
  ///   about the file.
  /// </summary>
  public sealed class FileInfoEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The size of the file being extracted in bytes.
    /// </summary>
    public long FileSizeInBytes
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    /// <summary>
    ///   The name of the file being extracted.
    /// </summary>
    public string FileName
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _message.str;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="FileInfoEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal FileInfoEventData( EventMessage message )
      : base( EventType.FileInfo, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "overwrite?" event.
  ///   This event occurs when a file that is being extracted already exists at the destination path.
  ///   This event expects a return value of ([y]es,[n]o,[a]lways,[s]never,[q]uit).
  /// </summary>
  /// <remarks>
  ///   This is handled by the main Unarc wrapper class using delegates.
  ///   This EventData class is mostly a formality.
  /// </remarks>
  public sealed class OverwriteFileRequestEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The size of the file that will be overwritten in bytes.
    /// </summary>
    public long FileSizeInBytes
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    /// <summary>
    ///   The name of the file that will be overwritten.
    /// </summary>
    public string FileName
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _message.str;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="OverwriteFileRequestEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal OverwriteFileRequestEventData( EventMessage message )
      : base( EventType.OverwriteFileRequest, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "password?" event.
  ///   This event requests the archive's password and expects either the supplied password buffer
  ///   to be filled. Additionally, a return value of ([y]es,[n]o,[q]uit) to be specified.
  /// </summary>
  /// <remarks>
  ///   This is handled by the main Unarc wrapper class using delegates.
  ///   This EventData class is mostly a formality.
  /// </remarks>
  public sealed class PasswordRequestEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The size of the password buffer.
    /// </summary>
    public int PasswordBufferSize
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _message.int1;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="PasswordRequestEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal PasswordRequestEventData( EventMessage message )
      : base( EventType.PasswordRequest, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "error" event.
  ///   This event informs you that an error has occured.
  /// </summary>
  public sealed class ErrorEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The error code.
    /// </summary>
    public UnarcErrorCode ErrorCode
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => ( UnarcErrorCode ) _message.int1;
    }

    /// <summary>
    ///   The error message.
    /// </summary>
    public string ErrorMessage
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _message.str;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs an <see cref="ErrorEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal ErrorEventData( EventMessage message )
      : base( EventType.Error, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "total_files" event.
  ///   This event occurs during the [l]ist command and provides the total number
  ///   of files inside the archive.
  /// </summary>
  public sealed class ArchiveTotalFilesEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The total number of files in the archive.
    /// </summary>
    public int TotalFiles
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _message.int1;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="ArchiveTotalFilesEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal ArchiveTotalFilesEventData( EventMessage message )
      : base( EventType.ArchiveTotalFiles, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "origsize" event.
  ///   This event occurs during the [l]ist command and provides the 
  ///   total uncompressed size of the archive.
  /// </summary>
  public sealed class ArchiveUncompressedSizeEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The total uncompressed size of the archive in bytes.
    /// </summary>
    public long UncompressedSize
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="ArchiveUncompressedSizeEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal ArchiveUncompressedSizeEventData( EventMessage message )
      : base( EventType.ArchiveUncompressedSize, message )
    {
    }

    #endregion

  }

  /// <summary>
  ///   Event data for the "compsize" event.
  ///   This event occurs during the [l]ist command and provides the 
  ///   total compressed size of the archive.
  /// </summary>
  public sealed class ArchiveCompressedSizeEventData : EventData
  {

    #region Properties

    /// <summary>
    ///   The total uncompressed size of the archive in bytes.
    /// </summary>
    public long CompressedSize
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => MergeIntValuesToLong();
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="ArchiveCompressedSizeEventData" />.
    /// </summary>
    /// <param name="message">
    ///   The original event message sent from Unarc.
    /// </param>
    internal ArchiveCompressedSizeEventData( EventMessage message )
      : base( EventType.ArchiveCompressedSize, message )
    {
    }

    #endregion

  }

}
