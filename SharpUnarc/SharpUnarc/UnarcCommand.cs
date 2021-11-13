using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpUnarc
{

  /// <summary>
  ///   The Unarc command type.
  /// </summary>
  public enum UnarcCommandType
  {

    /// <summary>
    ///   Lists the archive's contents.
    ///   Unarc.dll's implementation only lists the file count and archive size.
    /// </summary>
    List = 'l',

    /// <summary>
    ///   Extracts the archive's files into the current directory.
    /// </summary>
    ExtractToCurrentDirectory = 'e',

    /// <summary>
    ///   Extracts the archive's files with pathnames.
    /// </summary>
    ExtractWithPathnames = 'x',

    /// <summary>
    ///   Tests the archive's integrity.
    /// </summary>
    Test = 't'

  }

  /// <summary>
  ///   A command for executing operations with Unarc.
  /// </summary>
  public class UnarcCommand
  {

    #region Properties

    /// <summary>
    ///   The command type.
    /// </summary>
    public UnarcCommandType CommandType { get; }

    /// <summary>
    ///   The archive's path.
    /// </summary>
    public string ArchivePath { get; set; }

    /// <summary>
    ///   A collection of filenames, filelists, or search patterns to use 
    ///   when performing the operation.
    /// </summary>
    public List<string> FileNames { get; }

    /// <summary>
    ///   The base directory inside the archive.
    ///   (-ap{Path})
    /// </summary>
    public string BaseDirectory { get; set; }

    /// <summary>
    ///   The destination path.
    ///   (-dp{Path})
    /// </summary>
    public string DestinationPath { get; set; }

    /// <summary>
    ///   The temporary files path.
    ///   (-w{Path})
    /// </summary>
    public string TempFilePath { get; set; }

    /// <summary>
    ///   The archive's password.
    ///   (-p{Pwd})
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///   The memory limit for decompression.
    ///   (-ld{Mem})
    /// </summary>
    public string MemoryLimit { get; set; }

    /// <summary>
    ///   Whether or not files should be overwritten.
    ///   (-o{+/-})
    /// </summary>
    /// <remarks>
    ///   Set as null to handle files individually.
    /// </remarks>
    public bool? OverwriteFiles { get; set; }

    /// <summary>
    ///   Whether or not to add the default extension to the archive name.
    ///   (--noarcext)
    /// </summary>
    public bool NoArcExtension { get; set; }

    /// <summary>
    ///   The arc.ini config file path.
    ///   (--cfg{Path})
    /// </summary>
    public string ConfigFilePath { get; set; }

    #endregion

    #region Constructor

    private UnarcCommand( UnarcCommandType commandType, string archivePath, IEnumerable<string> fileNames = null )
    {
      CommandType = commandType;

      // Defaults
      ArchivePath = archivePath;
      BaseDirectory = null;
      DestinationPath = null;
      TempFilePath = null;
      Password = null;
      MemoryLimit = null;
      OverwriteFiles = null;
      NoArcExtension = false;
      ConfigFilePath = null; // Path.Combine( Environment.CurrentDirectory, "arc.ini" );

      if ( fileNames == null )
        fileNames = Enumerable.Empty<string>();
      FileNames = new List<string>( fileNames );
    }

    /// <summary>
    ///   Creates a list (l) command. Unarc.dll's implementation only lists the file count and archive size.
    /// </summary>
    public static UnarcCommand List( string archivePath, string password = null, bool noArcExtension = false, IEnumerable<string> fileNames = null )
    {
      return new UnarcCommand( UnarcCommandType.List, archivePath, fileNames )
      {
        Password = password,
        NoArcExtension = noArcExtension
      };
    }

    /// <summary>
    ///   Creates an extract (e) command.
    /// </summary>
    public static UnarcCommand ExtractToCurrentDirectory( string archivePath, string baseDirectory = null, string destinationPath = null,
      string tempFilePath = null, string password = null, string memoryLimit = null, bool? overwriteFiles = null,
      bool noArcExtension = false, string configFilePath = null, IEnumerable<string> fileNames = null )
    {
      return new UnarcCommand( UnarcCommandType.ExtractToCurrentDirectory, archivePath, fileNames )
      {
        BaseDirectory = baseDirectory,
        DestinationPath = destinationPath,
        TempFilePath = tempFilePath,
        Password = password,
        MemoryLimit = memoryLimit,
        OverwriteFiles = overwriteFiles,
        NoArcExtension = noArcExtension,
        ConfigFilePath = configFilePath
      };
    }

    /// <summary>
    ///   Creates an extract (x) command.
    /// </summary>
    public static UnarcCommand ExtractWithPathnames( string archivePath, string baseDirectory = null, string destinationPath = null,
      string tempFilePath = null, string password = null, string memoryLimit = null, bool? overwriteFiles = null,
      bool noArcExtension = false, string configFilePath = null, IEnumerable<string> fileNames = null )
    {
      return new UnarcCommand( UnarcCommandType.ExtractWithPathnames, archivePath, fileNames )
      {
        BaseDirectory = baseDirectory,
        DestinationPath = destinationPath,
        TempFilePath = tempFilePath,
        Password = password,
        MemoryLimit = memoryLimit,
        OverwriteFiles = overwriteFiles,
        NoArcExtension = noArcExtension,
        ConfigFilePath = configFilePath
      };
    }

    /// <summary>
    ///   Creates a test (t) command.
    /// </summary>
    public static UnarcCommand Test( string archivePath, string tempFilePath = null, string password = null,
      string memoryLimit = null, bool noArcExtension = false, string configFilePath = null )
    {
      return new UnarcCommand( UnarcCommandType.Test, archivePath )
      {
        TempFilePath = tempFilePath,
        Password = password,
        MemoryLimit = memoryLimit,
        NoArcExtension = noArcExtension,
        ConfigFilePath = configFilePath
      };
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Creates an arguments list for executing the command.
    /// </summary>
    /// <returns>
    ///   The arguments list.
    /// </returns>
    internal string[] GetArgumentsList()
    {
      var argList = new List<string>();

      // Add command
      argList.Add( ( ( char ) CommandType ).ToString() );

      // Add Options
      if ( !string.IsNullOrWhiteSpace( BaseDirectory ) )
        argList.Add( $"-ap{BaseDirectory}" );
      if ( !string.IsNullOrWhiteSpace( DestinationPath ) )
        argList.Add( $"-dp{DestinationPath}" );
      if ( !string.IsNullOrWhiteSpace( TempFilePath ) )
        argList.Add( $"-w{TempFilePath}" );
      if ( !string.IsNullOrEmpty( Password ) )
        argList.Add( $"-p{Password}" );
      if ( !string.IsNullOrWhiteSpace( MemoryLimit ) )
        argList.Add( $"-ld{MemoryLimit}" );
      if ( OverwriteFiles.HasValue )
        argList.Add( $"-o{( OverwriteFiles.Value ? '+' : '-' )}" );
      if ( NoArcExtension )
        argList.Add( "--noarcext" );
      if ( !string.IsNullOrWhiteSpace( ConfigFilePath ) )
        argList.Add( $"--cfg{ConfigFilePath}" );

      // Add delimiter
      argList.Add( "--" );

      // Add archive path
      argList.Add( ArchivePath );

      // Add FileNames
      foreach ( var pattern in FileNames )
        argList.Add( pattern );

      return argList.ToArray();
    }

    #endregion

  }

}
