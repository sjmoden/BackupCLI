using System.IO.Abstractions.TestingHelpers;
using Backups.Actions;
using Backups.Actions.Evaluate;
using Backups.Infrastructure;
using Backups.Model;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class EvaluateFolderShould
{
    private IBackupSettingsReader _backupSettingsReader = null!;
    private MockFileSystem _mockFileSystem = null!;
    private readonly MockFileData _emptyMockFileData = new(string.Empty);

    private readonly BackupSettings _backupSettings = new()
    {
        TrackedFiles = new List<string>{TrackedFile1,TrackedFile2},
        TrackedFolders = new List<string>{TrackedFolders1, TrackedFolders2}
    };

    private const string FolderRoot = @"c:\holidays";
    private const string UntrackedFile1 = "file1.jpg";
    private const string UntrackedFile2 = "file2.jpg";
    private const string UntrackedFolder1 = "Folder1";
    private const string UntrackedFolder2 = "Folder2";
    private const string TrackedFile1 = "tfile1.jpg";
    private const string TrackedFile2 = "tfile2.jpg";
    private const string TrackedFolders1 = "tFolder1";
    private const string TrackedFolders2 = "tFolder2";

    [SetUp]
    public void SetUp()
    {
        _backupSettingsReader = A.Fake<IBackupSettingsReader>();
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $@"{FolderRoot}\{UntrackedFile1}", _emptyMockFileData },
            { $@"{FolderRoot}\{UntrackedFile2}", _emptyMockFileData },
            { $@"{FolderRoot}\{TrackedFile1}", _emptyMockFileData },
            { $@"{FolderRoot}\{TrackedFile2}", _emptyMockFileData },
            { $@"{FolderRoot}\{UntrackedFolder1}", new MockDirectoryData() },
            { $@"{FolderRoot}\{UntrackedFolder2}", new MockDirectoryData() },
            { $@"{FolderRoot}\{TrackedFolders1}", new MockDirectoryData() },
            { $@"{FolderRoot}\{TrackedFolders2}", new MockDirectoryData() },
            { @"c:\otherfolder\shouldnotappear.jpg", _emptyMockFileData },
            { $@"{FolderRoot}\{BackupSettingFilePath.BackupSettingsFileName}", _emptyMockFileData },
        });
        
        A.CallTo(() => _backupSettingsReader.Read(FolderRoot)).Returns(_backupSettings);
    }
    
    [Test]
    public void Throw_an_exception_when_the_folder_does_not_contain_backup_settings()
    {
        _mockFileSystem = new MockFileSystem();
        var evaluateFolder = new EvaluateFolder(_mockFileSystem, new BackupSettingFilePath(_mockFileSystem), _backupSettingsReader);
        Assert.Throws<BackupSettingsNotFoundException>(() => evaluateFolder.Evaluate(FolderRoot));
    }

    [Test]
    public void Return_all_untracked_files_in_folder()
    {
        var evaluateFolder = new EvaluateFolder(_mockFileSystem, new BackupSettingFilePath(_mockFileSystem), _backupSettingsReader);
        var result = evaluateFolder.Evaluate(FolderRoot);
        
        var expectedResult = new List<string>{UntrackedFile1,UntrackedFile2};
        
        Assert.That(result.UntrackedFiles, Is.EquivalentTo(expectedResult));
    }

    [Test]
    public void Return_all_tracked_files_in_folder()
    {
        var evaluateFolder = new EvaluateFolder(_mockFileSystem, new BackupSettingFilePath(_mockFileSystem), _backupSettingsReader);
        var result = evaluateFolder.Evaluate(FolderRoot);
        
        var expectedResult = new List<string>{TrackedFile1,TrackedFile2};
        
        Assert.That(result.TrackedFiles, Is.EquivalentTo(expectedResult));
    }

    [Test]
    public void Return_all_tracked_folders_in_folder()
    {
        var evaluateFolder = new EvaluateFolder(_mockFileSystem, new BackupSettingFilePath(_mockFileSystem), _backupSettingsReader);
        var result = evaluateFolder.Evaluate(FolderRoot);
        
        var expectedResult = new List<string>{TrackedFolders1,TrackedFolders2};
        
        Assert.That(result.TrackedFolders, Is.EquivalentTo(expectedResult));
    }

    [Test]
    public void Return_all_untracked_folders_in_folder()
    {
        var evaluateFolder = new EvaluateFolder(_mockFileSystem, new BackupSettingFilePath(_mockFileSystem), _backupSettingsReader);
        var result = evaluateFolder.Evaluate(FolderRoot);
        
        var expectedResult = new List<string>{UntrackedFolder1,UntrackedFolder2};
        Assert.That(result.UntrackedFolders, Is.EquivalentTo(expectedResult));
    }
}