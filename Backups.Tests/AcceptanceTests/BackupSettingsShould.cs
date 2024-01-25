using System.IO.Abstractions.TestingHelpers;
using Backups.Actions;
using Backups.Actions.Initialise;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.AcceptanceTests;

[TestFixture]
public class BackupSettingsShould
{
    private MockFileSystem _fileSystem = null!;
    private const string ExistingFolder = @"C:\Example";

    [SetUp]
    public void CreateMockFileSystem()
    {
        _fileSystem = new MockFileSystem();
        _fileSystem.AddDirectory(ExistingFolder);   
    }
    
    [Test]
    public void Return_the_correct_backup_settings_when_initialised_and_then_read()
    {
        var settingsToSave = new BackupSettings()
        {
            FileName = "holiday pics"
        };
        
        var initialiseRootFile = new BackupSettingsModifier(_fileSystem);
        initialiseRootFile.TryCreate(ExistingFolder, settingsToSave);
        
        var backupSettingsReader = new BackupSettingsReader(_fileSystem, new BackupSettingFilePath(_fileSystem));
        var returnedSettings = backupSettingsReader.Read(ExistingFolder);
        
        Assert.That(returnedSettings.FileName, Is.EqualTo(settingsToSave.FileName));
    }
}