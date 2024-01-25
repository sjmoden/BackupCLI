using Backups.Model;

namespace Backups.Tests.UnitTests.BackupSettingsTests;

[TestFixture]
public class BackupSettingsTrackedFilesShould
{
    [Test]
    public void Add_a_new_file_to_the_tracked_file_property()
    {
        const string fileName = "A";
        var backupSettings = new BackupSettings();
        backupSettings.AddTrackedFile(fileName);
        Assert.That(backupSettings.TrackedFiles, Has.One.EqualTo(fileName));
    }

    [Test]
    public void Not_track_file_if_already_tracked()
    {
        const string fileName = "A";
        var backupSettings = new BackupSettings();
        backupSettings.AddTrackedFile(fileName);
        backupSettings.AddTrackedFile(fileName);
        Assert.That(backupSettings.TrackedFiles, Has.One.EqualTo(fileName));
    }

    [Test]
    public void Return_false_when_file_does_not_exist_in_tracked_files()
    {
        var backupSettings = new BackupSettings()
        {
            TrackedFiles = new List<string>
            {
                "file1.jpg"
            }
        };
        var result = backupSettings.DoesFileExistInTrackedFilesList("file2.jpg");
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void Return_true_when_file_does_exist_in_tracked_files()
    {
        const string fileName = "file1.jpg";
        var backupSettings = new BackupSettings()
        {
            TrackedFiles = new List<string>
            {
                fileName
            }
        };
        var result = backupSettings.DoesFileExistInTrackedFilesList(fileName);
        Assert.That(result, Is.True);
    }
}