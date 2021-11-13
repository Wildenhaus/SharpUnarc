using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpUnarc.Events;

namespace SharpUnarc.Tests
{

  [TestClass]
  public class UnarcEventTests : UnarcTestsBase
  {

    [TestMethod]
    public void ArchiveSizeEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.ExtractToCurrentDirectory( TestArchivePath, destinationPath: TestEnvPath );
      var archiveSize = GetFileSizeInBytes( TestArchivePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<ArchiveSizeEventData>();
      eventData.Should().NotBeEmpty();

      eventData.First().ArchiveSizeInBytes.Should().Be( archiveSize );
    }

    [TestMethod]
    public void BytesReadEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.ExtractToCurrentDirectory( TestArchivePath, destinationPath: TestEnvPath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<BytesReadEventData>();
      eventData.Should().NotBeEmpty();

      // Check int->long conversion
      eventData.Should().NotContain( x => x.BytesRead < 0, "Integer should properly convert." );

      eventData.Last().BytesRead.Should().BeGreaterThan( 0 );
    }

    [TestMethod]
    public void BytesWrittenEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.ExtractToCurrentDirectory( TestArchivePath, destinationPath: TestEnvPath );
      var fileSize = GetFileSizeInBytes( TestFilePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<BytesWrittenEventData>();
      eventData.Should().NotBeEmpty();

      // Check int->long conversion
      eventData.Should().NotContain( x => x.BytesWritten < 0, "Integer should properly convert." );

      eventData.Last().BytesWritten.Should().Be( fileSize );
    }

    [TestMethod]
    public void FileInfoEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.ExtractToCurrentDirectory( TestArchivePath, destinationPath: TestEnvPath );
      var fileName = Path.GetFileName( TestFilePath );
      var fileSize = GetFileSizeInBytes( TestFilePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<FileInfoEventData>();
      eventData.Should().NotBeEmpty();

      var fileInfo = eventData.First();
      fileInfo.FileName.Should().Be( fileName );
      fileInfo.FileSizeInBytes.Should().Be( fileSize );
    }

    [TestMethod]
    public void ArchiveTotalFilesEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.List( TestArchivePath );
      const int FILE_COUNT = 1;

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<ArchiveTotalFilesEventData>();
      eventData.Should().NotBeEmpty();

      eventData.First().TotalFiles.Should().Be( FILE_COUNT );
    }

    [TestMethod]
    public void ArchiveUncompressedSizeEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.List( TestArchivePath );
      var uncompressedSize = GetFileSizeInBytes( TestFilePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<ArchiveUncompressedSizeEventData>();
      eventData.Should().NotBeEmpty();

      eventData.First().UncompressedSize.Should().Be( uncompressedSize );
    }

    [TestMethod]
    public void ArchiveCompressedSizeEventData_Produces_Proper_Data()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.List( TestArchivePath );
      var archiveSize = GetFileSizeInBytes( TestArchivePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var eventData = GetLoggedEventData<ArchiveCompressedSizeEventData>();
      eventData.Should().NotBeEmpty();

      eventData.First().CompressedSize.Should().BeInRange( 1, archiveSize );
    }

  }

}
