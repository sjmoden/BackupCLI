using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Backups.Actions;
using Backups.Actions.Track;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BackupSettingsTrackFolderShould
{
    private readonly string _settingsFilePath = BackupSettingFilePath.Build(Folder);
    private const string Folder = @"C:\";
    private const string SubFolderToAdd1 = "Photos";
    private const string SubFolderToAdd2 = "Photos2";
    [Test]
    public void Throw_BackupSettingsNotFoundException_when_settings_do_not_exists_in_directory()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(Folder,SubFolderToAdd1), new MockDirectoryData() },
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFolder(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        Assert.Throws<BackupSettingsNotFoundException>(() => trackFile.Track(SubFolderToAdd1, Folder));
    }
    
    [Test]
    public void Throw_DirectoryNotFoundException_when_file_does_not_exist()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _settingsFilePath, new MockFileData(string.Empty) }
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFolder(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        Assert.Throws<DirectoryNotFoundException>(() => trackFile.Track(SubFolderToAdd1, Folder));
    }
    
    [Test]
    public void Write_tracked_folder_to_backup_settings()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _settingsFilePath, new MockFileData(string.Empty) },
            { Path.Combine(Folder,SubFolderToAdd1), new MockDirectoryData() },
            { Path.Combine(Folder,SubFolderToAdd2), new MockDirectoryData() },
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFolder(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        trackFile.Track(SubFolderToAdd1, Folder);
        trackFile.Track(SubFolderToAdd2, Folder);

        var backupSettingContents = mockFileSystem.File.ReadAllText(_settingsFilePath);
        var backupSettings = JsonSerializer.Deserialize<BackupSettings>(backupSettingContents) ?? new BackupSettings();
        Assert.That(backupSettings.TrackedFolders, Is.EquivalentTo(new List<string>{SubFolderToAdd1,SubFolderToAdd2}));
    }
}