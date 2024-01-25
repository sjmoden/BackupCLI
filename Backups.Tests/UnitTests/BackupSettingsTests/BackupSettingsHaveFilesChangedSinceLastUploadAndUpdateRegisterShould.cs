using System.IO.Abstractions.TestingHelpers;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.UnitTests.BackupSettingsTests;

[TestFixture]
public class BackupSettingsHaveFilesChangedSinceLastUploadAndUpdateRegisterShould
{
    private const string FileName1 = "a.file";
    private const string FileName2 = "b.file";
    private const string Folder1 = "Folder";
    private const string Folder2 = "Folder2";
    private const string FileInFolderName1 = "c.file";
    private const string FileInFolderName2 = "d.file";
    private const string FileInFolderName3 = "e.file";
    private BackupSettings _backupSettings = null!;
    private readonly string _filePath1 = Path.Combine(Environment.CurrentDirectory, FileName1);
    private readonly string _filePath2 = Path.Combine(Environment.CurrentDirectory, FileName2);
    private readonly string _folderPath1 = Path.Combine(Environment.CurrentDirectory, Folder1);
    private readonly string _folderPath2 = Path.Combine(Environment.CurrentDirectory, Folder2);
    private string _fileInFolderNamePath1 = null!;
    private string _fileInFolderNamePath2 = null!;
    private string _fileInFolderNamePath3 = null!;
    private readonly MockFileData _mockFileDataWithSomeContents = new("some contents");
    private readonly string _folderAndFilePath1 = Path.Combine(Folder1,FileInFolderName1);
    private readonly string _folderAndFilePath2 = Path.Combine(Folder1,FileInFolderName2);
    private readonly string _folderAndFilePath3 = Path.Combine(Folder2,FileInFolderName3);

    [SetUp]
    public void SetUp()
    {
        _fileInFolderNamePath1 = Path.Combine(_folderPath1, FileInFolderName1);
        _fileInFolderNamePath2 = Path.Combine(_folderPath1, FileInFolderName2);
        _fileInFolderNamePath3 = Path.Combine(_folderPath2, FileInFolderName3);
    }
    
    [Test]
    public void Updates_the_stored_hashes_when_previously_empty_and_return_true()
    {
        _backupSettings = new BackupSettings()
        {
            TrackedFiles = new List<string>{FileName1,FileName2},
            TrackedFolders = new List<string>{Folder1, Folder2}
        };
        
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _filePath1, _mockFileDataWithSomeContents },
            { _filePath2, new MockFileData("other contents") },
            {_folderPath1, new MockDirectoryData()},
            {_folderPath2, new MockDirectoryData()},
            {_fileInFolderNamePath1, _mockFileDataWithSomeContents},
            {_fileInFolderNamePath2, _mockFileDataWithSomeContents},
            {_fileInFolderNamePath3, _mockFileDataWithSomeContents}
        });
        var checkSum = new CheckSum(mockFileSystem);
        var fileHash1 = new FileHash(FileName1, checkSum.Get(_filePath1));
        var fileHash2 = new FileHash(FileName2, checkSum.Get(_filePath2));
        var fileInFolderHash1 = new FileHash(_folderAndFilePath1, checkSum.Get(_fileInFolderNamePath1));
        var fileInFolderHash2 = new FileHash(_folderAndFilePath2, checkSum.Get(_fileInFolderNamePath2));
        var fileInFolderHash3 = new FileHash(_folderAndFilePath3, checkSum.Get(_fileInFolderNamePath3));
        
        var result = _backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(mockFileSystem);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.Count.EqualTo(5));
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.One.EqualTo(fileHash1));
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.One.EqualTo(fileHash2));
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.One.EqualTo(fileInFolderHash1));
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.One.EqualTo(fileInFolderHash2));
            Assert.That(_backupSettings.PreviousHashesOfFiles, Has.One.EqualTo(fileInFolderHash3));
        });
    }

    [Test]
    public void Return_false_when_all_hashes_match_and_no_changes_to_previous_hashes_are_made()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {_filePath1, _mockFileDataWithSomeContents},
            {_filePath2, _mockFileDataWithSomeContents},
            {_folderPath1, new MockDirectoryData()},
            {_folderPath2, new MockDirectoryData()},
            {_fileInFolderNamePath1, _mockFileDataWithSomeContents},
            {_fileInFolderNamePath2, _mockFileDataWithSomeContents},
            {_fileInFolderNamePath3, _mockFileDataWithSomeContents}
        });

        var checkSum = new CheckSum(mockFileSystem);

        var expectedPreviousHashesOfFiles=new List<FileHash>
        {
            new(FileName1, checkSum.Get(_filePath1)),
            new(FileName2, checkSum.Get(_filePath2)),
            new(_folderAndFilePath1, checkSum.Get(_fileInFolderNamePath1)),
            new(_folderAndFilePath2, checkSum.Get(_fileInFolderNamePath2)),
            new(_folderAndFilePath3, checkSum.Get(_fileInFolderNamePath3))
        };
        
        var backupSettings = new BackupSettings
        {
            PreviousHashesOfFiles = new List<FileHash>(expectedPreviousHashesOfFiles),
            TrackedFiles = new List<string>{FileName1,FileName2},
            TrackedFolders = new List<string>{Folder1,Folder2}
        };

        var result = backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(mockFileSystem);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(false));
            Assert.That(backupSettings.PreviousHashesOfFiles, Is.EquivalentTo(expectedPreviousHashesOfFiles));
        });
    }
    
    [Test]
    public void Return_true_when_a_hash_does_not_match_and_update_previous_hashes()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {_filePath1, _mockFileDataWithSomeContents},
            {_filePath2, _mockFileDataWithSomeContents},
        });

        var checkSum = new CheckSum(mockFileSystem);

        var startingFileHash=new List<FileHash>
        {
            new(FileName1, checkSum.Get(_filePath1)),
            new(FileName2, checkSum.Get(_filePath2)),
        };
        
        mockFileSystem.File.WriteAllText(_filePath2, "new line");

        var expectedFileHash = new List<FileHash>
        {
            startingFileHash.Single(f => f.FileName.Equals(FileName1)),
            new(FileName2, checkSum.Get(_filePath2))
        };
        
        var backupSettings = new BackupSettings
        {
            PreviousHashesOfFiles = new List<FileHash>(startingFileHash),
            TrackedFiles = startingFileHash.Select(p => p.FileName).ToList()
        };

        var result = backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(mockFileSystem);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(true));
            Assert.That(backupSettings.PreviousHashesOfFiles, Is.EquivalentTo(expectedFileHash));
        });
    }
    
    [Test]
    public void Return_true_when_a_hash_does_not_match_and_update_previous_hashes_for_file_in_sub_folder()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {_fileInFolderNamePath1, _mockFileDataWithSomeContents},
            {_fileInFolderNamePath2, _mockFileDataWithSomeContents},
        });

        var checkSum = new CheckSum(mockFileSystem);

        var startingFileHash=new List<FileHash>
        {
            new(_folderAndFilePath1, checkSum.Get(_fileInFolderNamePath1)),
            new(_folderAndFilePath2, checkSum.Get(_fileInFolderNamePath2)),
        };
        
        mockFileSystem.File.WriteAllText(_fileInFolderNamePath2, "new line");

        var expectedFileHash = new List<FileHash>
        {
            startingFileHash.Single(f => f.FileName.Equals(_folderAndFilePath1)),
            new(_folderAndFilePath2, checkSum.Get(_fileInFolderNamePath2))
        };
        
        var backupSettings = new BackupSettings
        {
            PreviousHashesOfFiles = new List<FileHash>(startingFileHash),
            TrackedFolders = new List<string>{Folder1}
        };

        var result = backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(mockFileSystem);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(true));
            Assert.That(backupSettings.PreviousHashesOfFiles, Is.EquivalentTo(expectedFileHash));
        });
    }

    [Test]
    public void Throw_exception_when_tracked_file_is_not_found()
    {
        _backupSettings = new BackupSettings
        {
            TrackedFiles = new List<string> { FileName1 }
        };
        
        Assert.Throws<FileNotFoundException>(() =>_backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(new MockFileSystem()));
    }
    
    [Test]
    public void Throw_exception_when_tracked_folder_is_not_found()
    {
        _backupSettings = new BackupSettings
        {
            TrackedFolders = new List<string> { Folder1 }
        };
        
        Assert.Throws<DirectoryNotFoundException>(() =>_backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister(new MockFileSystem()));
    }
}