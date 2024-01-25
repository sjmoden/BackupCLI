using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.UnitTests.BackupSettingsTests;

[TestFixture]
public class BackupSettingsGetTrackedFilesAndFoldersShould
{
    [Test]
    public void Throw_an_exception_when_tracked_files_are_empty_when_get_tracked_files_is_called()
    {
        var backupSettings = new BackupSettings();
        Assert.Throws<NoTrackedFilesOrFoldersException>(() => backupSettings.GetTrackedFilesAndFoldersThrowIfNotFound());
    }
    
    [Test]
    public void Return_tracked_files_and_folders_when_get_tracked_files_is_called()
    {
        var trackedFiles = new List<string>{"abc", "def"};
        var trackedFolders = new List<string>{"folder1", "folder2"};

        var expectedResult = trackedFiles.Union(trackedFolders);
        
        var backupSettings = new BackupSettings
        {
            TrackedFiles = trackedFiles,
            TrackedFolders = trackedFolders
        };
        Assert.That(backupSettings.GetTrackedFilesAndFoldersThrowIfNotFound(), Is.EquivalentTo(expectedResult));
    }
}