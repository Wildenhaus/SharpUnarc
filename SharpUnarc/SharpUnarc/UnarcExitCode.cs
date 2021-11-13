namespace SharpUnarc
{

  /// <summary>
  ///   Unarc exit codes.
  /// </summary>
  public enum UnarcExitCode : int
  {

    /// <summary>
    ///   Operation successfully completed.
    /// </summary>
    ExitSuccess = 0x00,

    /// <summary>
    ///   Operation failed.
    /// </summary>
    ExitFailure = 0x01,

    /// <summary>
    ///   Operation failed. Unarc encountered a serious error.
    /// </summary>
    ExitSeriousFailure = 0x02,

  }

}
