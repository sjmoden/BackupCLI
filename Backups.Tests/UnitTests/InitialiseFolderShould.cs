using Backups.Actions;
using Backups.Actions.Initialise;
using Backups.Model;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class InitialiseFolderShould
{
    [Test]
    public void Create_backup_settings()
    {
        var initialiseOptions = new InitialiseOptions
        {
            ZipFileName = "abc",
            AzureStorageAccountName = "account123",
            Container = "backup"
        };
        
        var backupSettingsCreator = A.Fake<IBackupSettingsModifier>();

        var initialiseFolder = new InitialiseFolder(backupSettingsCreator);
        initialiseFolder.Initialise(initialiseOptions);

        A.CallTo(() =>
            backupSettingsCreator.TryCreate(Environment.CurrentDirectory,
                new BackupSettings { FileName = initialiseOptions.ZipFileName, AzureStorageAccountName = initialiseOptions.AzureStorageAccountName,Container = initialiseOptions.Container}
                )).MustHaveHappenedOnceExactly();
    }
}