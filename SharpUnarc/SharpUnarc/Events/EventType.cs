using System.ComponentModel;

namespace SharpUnarc.Events
{

  public enum EventType : byte
  {

    /// <summary>
    ///   An unknown event type.
    ///   This should never be the case unless a new version of Unarc is released.
    /// </summary>
    Unknown,

    /// <summary>
    ///   An event that occurs at the start of unpacking and provides the total archive size.
    /// </summary>
    [Description( "total" )]
    ArchiveSize,

    /// <summary>
    ///   An event that occurs when bytes have been read from the archive.
    /// </summary>
    [Description( "read" )]
    BytesRead,

    /// <summary>
    ///   An event that occurs when bytes have been written to extracted files.
    /// </summary>
    [Description( "write" )]
    BytesWritten,

    /// <summary>
    ///   An event that occurs when a file is about to be extracted and provides information
    ///   about the file.
    /// </summary>
    [Description( "filename" )]
    FileInfo,

    /// <summary>
    ///   An event that occurs when an extraced file will overwrite a file that already
    ///   exists in the destination path.
    /// </summary>
    [Description( "overwrite?" )]
    OverwriteFileRequest,

    /// <summary>
    ///   An event that occurs when an archive is password protected.
    /// </summary>
    [Description( "password?" )]
    PasswordRequest,

    /// <summary>
    ///   An event that occurs when an error is encountered.
    /// </summary>
    [Description( "error" )]
    Error,

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total number
    ///   of files inside the archive.
    /// </summary>
    [Description( "total_files" )]
    ArchiveTotalFiles,

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total 
    ///   uncompressed size of the archive.
    /// </summary>
    [Description( "origsize" )]
    ArchiveUncompressedSize,

    /// <summary>
    ///   An event that occurs during the [l]ist command and provides the total 
    ///   uncompressed size of the archive.
    /// </summary>
    [Description( "compsize" )]
    ArchiveCompressedSize,

  }

}
