using System.IO.Abstractions.TestingHelpers;
using Backups.Actions.Archive;
using Backups.Infrastructure;
using Backups.Model;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class TempArchiveZipShould
{
    private const string File1 = "file.jpg";
    private const string File2 = "file2.jpg";
    private const string Password = "password";
    private readonly string _newGuid = Guid.NewGuid().ToString();
    private ISevenZip _sevenZip = null!;
    private readonly IGuidCreator _guidCreator = A.Fake<IGuidCreator>();
    private BackupSettings _backupSettings = null!;
    private string _tempFilePath = null!;
    private MockFileSystem _mockFileSystem = null!;

    [SetUp]
    public void SetUp()
    {
        _sevenZip = A.Fake<ISevenZip>();
        A.CallTo(() => _guidCreator.NewGuid()).Returns(_newGuid);
        
        _backupSettings = new BackupSettings
        {
            TrackedFiles = new List<string>
            {
                File1,File2
            },
            FileName = "abc"
        };
        
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"{_backupSettings.FileName}_{DateTime.Now:yyyyMMddHHss}_{_newGuid}.7z");
        _mockFileSystem = new MockFileSystem();
    }

    [Test]
    public void Create_zip_all_files_in_settings_to_a_temp_zip_file()
    {
        var tempArchiveZip = new TempArchiveZip(_guidCreator, _sevenZip, _mockFileSystem);
        tempArchiveZip.ZipFileToTempFolder(_backupSettings,Password);

        A.CallTo(() => _sevenZip.AddFileToZip(new AddFileToZipParameters(Password,_tempFilePath,File1)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _sevenZip.AddFileToZip(new AddFileToZipParameters(Password, _tempFilePath, File2)))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Throw_exception_if_there_are_no_files_to_track()
    {
        var backupSettings = new BackupSettings();
        var tempArchiveZip = new TempArchiveZip(_guidCreator, _sevenZip, _mockFileSystem);
        Assert.Throws<NoTrackedFilesOrFoldersException>(() => tempArchiveZip.ZipFileToTempFolder(backupSettings,"password"));
    }

    [Test]
    public void Return_temp_zip_file_path()
    {
        var tempArchiveZip = new TempArchiveZip(_guidCreator, _sevenZip, _mockFileSystem);
        var returnedPath = tempArchiveZip.ZipFileToTempFolder(_backupSettings,Password);
        
        Assert.That(returnedPath, Is.EqualTo(_tempFilePath));
    }

    [Test]
    public void Delete_the_temp_file()
    {
        const string cTempFile = @"c:\temp.file";
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            { { cTempFile, new MockFileData("content") } });

        Assert.That(_mockFileSystem.File.Exists(cTempFile), Is.True);
        
        var tempArchiveZip = new TempArchiveZip(_guidCreator, _sevenZip, _mockFileSystem);
        tempArchiveZip.DeleteTempZip(cTempFile);
        
        Assert.That(_mockFileSystem.File.Exists(cTempFile), Is.False);
    }
}