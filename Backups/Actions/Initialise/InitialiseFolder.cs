using Backups.Model;

namespace Backups.Actions.Initialise;

public class InitialiseFolder
{
    private readonly IBackupSettingsModifier _backupSettingsModifier;

    public InitialiseFolder(IBackupSettingsModifier backupSettingsModifier)
    {
        _backupSettingsModifier = backupSettingsModifier;
    }

    public void Initialise(InitialiseOptions initialiseOptions)
    {
        _backupSettingsModifier.TryCreate(Environment.CurrentDirectory,
            new BackupSettings
            {
                FileName = initialiseOptions.ZipFileName,
                AzureStorageAccountName = initialiseOptions.AzureStorageAccountName,
                Container = initialiseOptions.Container
            });
    }
}