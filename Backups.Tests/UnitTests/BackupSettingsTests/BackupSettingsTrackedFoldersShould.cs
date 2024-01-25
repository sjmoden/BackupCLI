using Backups.Model;

namespace Backups.Tests.UnitTests.BackupSettingsTests;

[TestFixture]
public class BackupSettingsTrackedFoldersShould
{
    private const string FolderName = "A";
    private readonly BackupSettings _backupSettings = new BackupSettings();
    
    [Test]
    public void Add_a_new_file_to_the_tracked_folder_list()
    {
        _backupSettings.AddTrackedFolder(FolderName);
        Assert.That(_backupSettings.TrackedFolders, Has.One.EqualTo(FolderName));
    }
    
    [Test]
    public void Not_track_folder_if_already_tracked()
    {
        _backupSettings.AddTrackedFolder(FolderName);
        _backupSettings.AddTrackedFolder(FolderName);
        Assert.That(_backupSettings.TrackedFolders, Has.One.EqualTo(FolderName));
    }
    
    [Test]
    public void Return_false_when_folder_does_not_exist_in_tracked_folders()
    {
        var backupSettings = new BackupSettings()
        {
            TrackedFolders = new List<string>
            {
                "folder"
            }
        };
        var result = backupSettings.DoesFolderExistInTrackedFolderList("folder2");
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void Return_true_when_folder_does_exist_in_tracked_folders()
    {
        const string folderName = "folder";
        var backupSettings = new BackupSettings()
        {
            TrackedFolders = new List<string>
            {
                folderName
            }
        };
        var result = backupSettings.DoesFolderExistInTrackedFolderList(folderName);
        Assert.That(result, Is.True);
    }
}