namespace SharpUnarc.Events
{

  /// <summary>
  ///   Response values for a file overwrite request.
  /// </summary>
  public enum OverwriteFileRequestResponse : int
  {

    /// <summary>
    ///   Allow the file to be overwritten.
    /// </summary>
    Yes = 'y',

    /// <summary>
    ///   Don't allow the file to be overwritten.
    /// </summary>
    No = 'n',

    /// <summary>
    ///   Always allow files to be overwritten (stop asking).
    /// </summary>
    Always = 'a',

    /// <summary>
    ///   Never allow files to be overwritten (stop asking).
    /// </summary>
    Never = 'n',

    /// <summary>
    ///   Abort the extraction.
    /// </summary>
    Abort = 'q'

  }

}
