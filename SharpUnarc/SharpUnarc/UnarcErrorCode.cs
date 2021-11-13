namespace SharpUnarc
{

  /// <summary>
  ///   Unarc error codes.
  /// </summary>
  public enum UnarcErrorCode : int
  {

    /// <summary>
    ///   Operation was successful.
    /// </summary>
    Ok = 0,

    /// <summary>
    ///   General decompression error.
    /// </summary>
    GeneralError = -1,

    /// <summary>
    ///   Invalid compression method or parameters.
    /// </summary>
    InvalidCompressorError = -2,

    /// <summary>
    ///   Attempted to compress with Unarc, which is decompress only.
    /// </summary>
    OnlyDecompress = -3,

    /// <summary>
    ///   Output block size in decompression memory is not enough for all output data.
    /// </summary>
    OutBlockTooSmall = -4,

    /// <summary>
    ///   Can't allocate the memory needed for decompression.
    /// </summary>
    NotEnoughMemory = -5,

    /// <summary>
    ///   Error while reading data.
    /// </summary>
    ErrorRead = -6,

    /// <summary>
    ///   Data can't be decompressed.
    /// </summary>
    BadCompressedData = -7,

    /// <summary>
    ///   Requested feature isn't supported.
    /// </summary>
    NotImplemented = -8,

    /// <summary>
    ///   Required data was already decompressed.
    /// </summary>
    NoMoreDataRequired = -9,

    /// <summary>
    ///   Operation terminated by the user.
    /// </summary>
    OperationTerminated = -10,

    /// <summary>
    ///   Error while writing data.
    /// </summary>
    ErrorWrite = -11,

    /// <summary>
    ///   File failed CRC check.
    /// </summary>
    BadCRC = -12,

    /// <summary>
    ///   Invalid password was provided.
    /// </summary>
    BadPassword = -13,

    /// <summary>
    ///   Archive headers are corrupted.
    /// </summary>
    BadHeaders = -14,

    /// <summary>
    ///   Unarc encountered an internal error.
    /// </summary>
    Internal = -15,

  }

}
