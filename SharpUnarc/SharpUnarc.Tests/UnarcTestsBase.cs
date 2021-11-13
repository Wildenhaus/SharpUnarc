using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpUnarc.Events;

namespace SharpUnarc.Tests
{

  public abstract class UnarcTestsBase
  {

    #region Constants

    private const string PATH_TESTENV = "TestEnv";
    private const string PATH_TESTDATA = "TestData";

    #endregion

    #region Data Members

    private readonly Unarc _unarc;
    private readonly List<EventData> _eventDataLog;

    #endregion

    #region Properties

    public Unarc Unarc
    {
      get => _unarc;
    }

    public IReadOnlyList<EventData> EventDataLog
    {
      get => _eventDataLog;
    }

    public string TestEnvPath
    {
      get => Path.Combine( Environment.CurrentDirectory, PATH_TESTENV );
    }

    public string TestFilePath
    {
      get => Path.Combine( Environment.CurrentDirectory, PATH_TESTDATA, "TestFile.txt" );
    }

    public string TestArchivePath
    {
      get => Path.Combine( Environment.CurrentDirectory, PATH_TESTDATA, "TestArchive.arc" );
    }

    #endregion

    #region Constructor

    protected UnarcTestsBase()
    {
      _unarc = new Unarc();
      _eventDataLog = new List<EventData>();
      _unarc.EventReceived += ( s, e ) => _eventDataLog.Add( e );

      PrepareTestingEnvironment();
    }

    #endregion

    #region Helper Methods

    protected UnarcExitCode ExecuteCommand( UnarcCommand command )
      => _unarc.ExecuteCommand( command );

    protected IEnumerable<TEventData> GetLoggedEventData<TEventData>()
      where TEventData : EventData
      => _eventDataLog.Where( x => x is TEventData ).Cast<TEventData>();

    protected long GetFileSizeInBytes( string file )
      => new FileInfo( file ).Length;

    #endregion

    #region Private Methods

    private void PrepareTestingEnvironment()
    {
      if ( Directory.Exists( TestEnvPath ) )
        Directory.Delete( TestEnvPath, true );

      Directory.CreateDirectory( TestEnvPath );

      if ( !File.Exists( TestFilePath ) )
        GenerateTestFile();
    }

    private void GenerateTestFile()
    {
      const long TEST_FILE_SIZE = ( long ) int.MaxValue * 2;
      using ( var writer = File.CreateText( TestFilePath ) )
      {
        while ( writer.BaseStream.Position < TEST_FILE_SIZE )
          writer.Write( "FOOBAR" );
      }
    }

    #endregion

  }

}
