using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Backups.Actions;
using Backups.Actions.Track;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BackupSettingsTrackFileShould
{
    private readonly string _settingsFilePath = BackupSettingFilePath.Build(Folder);
    private const string Folder = @"C:\";
    private const string File1JpgName = "file1.jpg";
    private const string File2JpgName = "file2.jpg";

    [Test]
    public void Throw_BackupSettingsNotFoundException_when_settings_do_not_exists_in_directory()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(Folder,File1JpgName), new MockFileData(string.Empty) },
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFile(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        Assert.Throws<BackupSettingsNotFoundException>(() => trackFile.Track(File1JpgName, Folder));
    }

    [Test]
    public void Throw_FileNotFoundException_when_file_does_not_exist()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _settingsFilePath, new MockFileData(string.Empty) }
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFile(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        Assert.Throws<FileNotFoundException>(() => trackFile.Track(File1JpgName, Folder));
    }

    [Test]
    public void Write_tracked_files_to_backup_settings()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _settingsFilePath, new MockFileData(string.Empty) },
            { Path.Combine(Folder,File1JpgName), new MockFileData(string.Empty) },
            { Path.Combine(Folder,File2JpgName), new MockFileData(string.Empty) },
        });
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        var trackFile = new BackupSettingsTrackFile(mockFileSystem, new BackupSettingsReader(mockFileSystem, backupSettingFilePath), new BackupSettingsModifier(mockFileSystem));
        trackFile.Track(File1JpgName, Folder);
        trackFile.Track(File2JpgName, Folder);

        var backupSettingContents = mockFileSystem.File.ReadAllText(_settingsFilePath);
        var backupSettings = JsonSerializer.Deserialize<BackupSettings>(backupSettingContents) ?? new BackupSettings();
        Assert.That(backupSettings.GetTrackedFilesAndFoldersThrowIfNotFound(), Is.EquivalentTo(new List<string>{File1JpgName,File2JpgName}));
    }
}