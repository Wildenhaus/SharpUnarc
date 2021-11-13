namespace SharpUnarc.Events
{

  /// <summary>
  ///   An event message received from Unarc.
  /// </summary>
  public struct EventMessage
  {

    #region Data Members

    /// <summary>
    ///   The event type.
    /// </summary>
    public readonly string what;

    /// <summary>
    ///   The first numerical value.
    /// </summary>
    public readonly int int1;

    /// <summary>
    ///   The second numerical value.
    /// </summary>
    public readonly int int2;

    /// <summary>
    ///   The string value.
    /// </summary>
    public readonly string str;

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="EventMessage" />.
    /// </summary>
    public EventMessage( string what, int int1, int int2, string str )
    {
      this.what = what;
      this.int1 = int1;
      this.int2 = int2;
      this.str = str;
    }

    #endregion

  }

}
