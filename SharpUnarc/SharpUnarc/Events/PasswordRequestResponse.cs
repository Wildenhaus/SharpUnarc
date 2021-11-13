namespace SharpUnarc.Events
{

  /// <summary>
  ///   Response values for an archive password request.
  /// </summary>
  public enum PasswordRequestResponse : int
  {

    /// <summary>
    ///   The password has been provided.
    /// </summary>
    Provided = 'y',

    /// <summary>
    ///   The password has NOT been provided.
    /// </summary>
    NotProvided = 'n',

    /// <summary>
    ///   Abort extraction.
    /// </summary>
    Abort = 'q'

  }

}
