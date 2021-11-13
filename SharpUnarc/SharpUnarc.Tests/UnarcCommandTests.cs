using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpUnarc.Events;

namespace SharpUnarc.Tests
{

  [TestClass]
  public class UnarcCommandTests : UnarcTestsBase
  {

    [TestMethod]
    public void Extract_command_produces_expected_results()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.ExtractToCurrentDirectory( TestArchivePath, destinationPath: TestEnvPath );
      var archiveSize = GetFileSizeInBytes( TestArchivePath );
      var fileSize = GetFileSizeInBytes( TestFilePath );
      var fileName = Path.GetFileName( TestFilePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var archiveSizeData = GetLoggedEventData<ArchiveSizeEventData>().First();
      archiveSizeData.ArchiveSizeInBytes.Should().Be( archiveSize );

      var fileInfoData = GetLoggedEventData<FileInfoEventData>().First();
      fileInfoData.FileSizeInBytes.Should().Be( fileSize );
      fileInfoData.FileName.Should().Be( fileName );

      var bytesReadData = GetLoggedEventData<BytesReadEventData>();
      bytesReadData.Should().NotBeEmpty();

      var bytesWrittenData = GetLoggedEventData<BytesWrittenEventData>();
      bytesWrittenData.Last().BytesWritten.Should().Be( fileSize );
    }

    [TestMethod]
    public void List_command_produces_expected_results()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.List( TestArchivePath );

      const int TOTAL_ARCHIVE_FILES = 1;
      var uncompressedSize = GetFileSizeInBytes( TestFilePath );
      var compressedSize = GetFileSizeInBytes( TestArchivePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var totalFilesData = GetLoggedEventData<ArchiveTotalFilesEventData>().First();
      totalFilesData.TotalFiles.Should().Be( TOTAL_ARCHIVE_FILES );

      var uncompressedSizeData = GetLoggedEventData<ArchiveUncompressedSizeEventData>().First();
      uncompressedSizeData.UncompressedSize.Should().Be( uncompressedSize );

      var compressedSizeData = GetLoggedEventData<ArchiveCompressedSizeEventData>().First();
      compressedSizeData.CompressedSize.Should().BeInRange( 1, compressedSize );
    }

    [TestMethod]
    public void Test_command_produces_expected_results()
    {
      //========================================
      // Arrange
      //========================================
      var command = UnarcCommand.Test( TestArchivePath );

      const int TOTAL_ARCHIVE_FILES = 1;
      var uncompressedSize = GetFileSizeInBytes( TestFilePath );
      var compressedSize = GetFileSizeInBytes( TestArchivePath );

      //========================================
      // Act
      //========================================
      ExecuteCommand( command );

      //========================================
      // Assert
      //========================================
      var totalFilesData = GetLoggedEventData<ArchiveTotalFilesEventData>().First();
      totalFilesData.TotalFiles.Should().Be( TOTAL_ARCHIVE_FILES );

      var uncompressedSizeData = GetLoggedEventData<ArchiveUncompressedSizeEventData>().First();
      uncompressedSizeData.UncompressedSize.Should().Be( uncompressedSize );

      var compressedSizeData = GetLoggedEventData<ArchiveCompressedSizeEventData>().First();
      compressedSizeData.CompressedSize.Should().BeInRange( 1, compressedSize );
    }

  }

}
