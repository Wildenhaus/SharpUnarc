using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpUnarc
{

  public partial class Unarc
  {

    #region Constants

    /// <summary>
    ///   The minimum length of an argument array.
    /// </summary>
    private const int MIN_ARGUMENTS_LENGTH = 10;

    #endregion

    #region DLL Imports

    /// <summary>
    ///   Extracts an ARC file.
    /// </summary>
    /// <param name="__arglist">
    ///   The list of arguments.
    ///   FreeArcExtract( UnarcCallback, ...string ).
    /// </param>
    /// <returns>
    ///   An integer denoting the result status.
    /// </returns>
    /// <remarks>
    ///   We only have __arglist as a parameter. This is because we're dynamically
    ///   creating the arglist based on an array of strings. See DynamicCallFreeArcExtract method.
    ///   
    ///   The number of strings in the arglist following the callback must be >= 10(?).
    ///   Any unused strings must be empty and not null.
    ///   If this condition is not met, Unarc.dll will attempt to read from an invalid memory address.
    /// </remarks>
    [DllImport( "unarc.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, SetLastError = true )]
    internal static extern int FreeArcExtract( __arglist );

    #endregion

    #region Constructor

    static Unarc()
    {
      Enforce32BitMode();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///   Generates a dynamic call to FreeArcExtract by generating an arglist.
    /// </summary>
    /// <param name="callback">
    ///   The <see cref="UnarcCallbackHandler" /> to handle messages.
    /// </param>
    /// <param name="args">
    ///   The arguments to use for extraction.
    /// </param>
    /// <returns>
    ///   The result code of FreeArcExtract.
    /// </returns>
    private static UnarcExitCode DynamicCallFreeArcExtract( UnarcCallbackHandler callbackHandler, IEnumerable<string> arguments )
    {
      var args = EnsureArgArrayLength( arguments, MIN_ARGUMENTS_LENGTH );

      var method = new DynamicMethod(
        name: "DynamicCallFreeArcExtract",
        returnType: typeof( int ),
        parameterTypes: new Type[] { typeof( UnarcCallbackHandler ) },
        owner: typeof( Unarc ),
        skipVisibility: true );

      var il = method.GetILGenerator();
      {
        // Load the callback as the first parameter.
        il.Emit( OpCodes.Ldarg_0 );

        // Load the string arglist.
        foreach ( var arg in args )
          il.Emit( OpCodes.Ldstr, arg );

        // Call FreeArcExtract.
        var externMethod = typeof( Unarc ).GetMethod( nameof( FreeArcExtract ), BindingFlags.NonPublic | BindingFlags.Static );

        var paramTypeList = Enumerable.Repeat( typeof( string ), args.Length + 1 ).ToArray();
        paramTypeList[ 0 ] = typeof( UnarcCallbackHandler );

        il.EmitCall( OpCodes.Call, externMethod, paramTypeList );

        // Return the status code.
        il.Emit( OpCodes.Ret );
      }

      var compiledCall = ( Func<UnarcCallbackHandler, int> ) method.CreateDelegate( typeof( Func<UnarcCallbackHandler, int> ) );
      return ( UnarcExitCode ) compiledCall.Invoke( callbackHandler );
    }

    /// <summary>
    ///   Ensures the arguments array is a minimum length, padding with empty
    ///   strings if needed.
    /// </summary>
    /// <param name="args">
    ///   The command arguments.
    /// </param>
    /// <param name="minLength">
    ///   The minimum length of the arguments array.
    /// </param>
    /// <returns>
    ///   An arguments array with a correct minimum length.
    /// </returns>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static string[] EnsureArgArrayLength( IEnumerable<string> args, int minLength = MIN_ARGUMENTS_LENGTH )
    {
      var stringsNeeded = minLength - args.Count();
      if ( stringsNeeded <= 0 )
        return args.ToArray();

      var concatArgs = Enumerable.Repeat( string.Empty, stringsNeeded );
      return args.Concat( concatArgs ).ToArray();
    }

    /// <summary>
    ///   Unarc.dll only supports 32-bit processes.
    ///   Enforce this.
    /// </summary>
    private static void Enforce32BitMode()
    {
      if ( Environment.Is64BitProcess )
        throw new NotSupportedException( 
          "Unarc.dll is only compatible with 32-bit applications. " +
          "Please ensure you are targeting x86 in your project settings." 
          );
    }

    #endregion

  }

}
