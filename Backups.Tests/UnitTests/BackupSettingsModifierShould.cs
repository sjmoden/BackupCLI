using System.IO.Abstractions.TestingHelpers;
using Backups.Actions;
using Backups.Model;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BackupSettingsModifierShould
{
    private MockFileSystem _fileSystem = null!;
    private readonly BackupSettings _backUpSettings = new()
    {
        FileName = "holiday pics",
        AzureStorageAccountName = "account123",
        Container = "backUp"
    };
    private const string ExistingFolder = @"C:\Example";
    private const string BackupSettingsFileName = ".backupSettings";
    private const string FileToCreatePath = $@"{ExistingFolder}\{BackupSettingsFileName}";

    [SetUp]
    public void CreateMockFileSystem()
    {
        _fileSystem = new MockFileSystem();
        _fileSystem.AddDirectory(ExistingFolder);   
    }
    
    [Test]
    public void DoesNotOverwriteExistingFile()
    {
        const string existingContent = "Here is some content";
        _fileSystem.AddFile(FileToCreatePath, new MockFileData(existingContent));
        
        var initialiseRootFile = new BackupSettingsModifier(_fileSystem);
        initialiseRootFile.TryCreate(ExistingFolder, new BackupSettings());
        
        Assert.That(_fileSystem.File.ReadAllText(FileToCreatePath), Is.EqualTo(existingContent));
    }
    
    [Test]
    public void ReturnFalseWhenFileAlreadyExists()
    {
        _fileSystem.AddFile(FileToCreatePath, new MockFileData(string.Empty));
        
        var initialiseRootFile = new BackupSettingsModifier(_fileSystem);
        var result = initialiseRootFile.TryCreate(ExistingFolder, new BackupSettings());
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ReturnTrueWhenInitialisingFolder()
    {
        var initialiseRootFile = new BackupSettingsModifier(_fileSystem);
        var result = initialiseRootFile.TryCreate(ExistingFolder, new BackupSettings());
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void CreateInitialiseFileInFolder()
    {
        var initialiseRootFile = new BackupSettingsModifier(_fileSystem);
        initialiseRootFile.TryCreate(ExistingFolder, new BackupSettings());

        Assert.That(_fileSystem.File.Exists(FileToCreatePath));
    }

    [Test]
    public void CreateFileWithSerialised_data()
    {
        var backupSettingsModifier = new BackupSettingsModifier(_fileSystem);
        backupSettingsModifier.TryCreate(ExistingFolder, _backUpSettings);
        
        var fileContents = _fileSystem.File.ReadAllText(FileToCreatePath);
        Assert.That(fileContents, Is.EqualTo(_backUpSettings.ToSerialisedString()));
    }

    [Test]
    public void Overwrite_backup_settings()
    {
        _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {FileToCreatePath, new MockFileData("ignore")}
        });
        
        var backupSettingsModifier = new BackupSettingsModifier(_fileSystem);
        backupSettingsModifier.Overwrite(ExistingFolder, _backUpSettings);
        
        var fileContents = _fileSystem.File.ReadAllText(FileToCreatePath);
        Assert.That(fileContents, Is.EqualTo(_backUpSettings.ToSerialisedString()));
    }
}