using System.IO.Abstractions;

namespace Backups.Actions.Track;

public class BackupSettingsTrackFolder:IBackupSettingsTrackFolder
{
    private readonly IFileSystem _fileSystem;
    private readonly IBackupSettingsReader _backupSettingsReader;
    private readonly IBackupSettingsModifier _backupSettingsModifier;

    public BackupSettingsTrackFolder(IFileSystem fileSystem, IBackupSettingsReader backupSettingsReader, IBackupSettingsModifier backupSettingsModifier)
    {
        _fileSystem = fileSystem;
        _backupSettingsReader = backupSettingsReader;
        _backupSettingsModifier = backupSettingsModifier;
    }

    public void Track(string subFolderToAdd, string folder)
    {
        if (!_fileSystem.Directory.Exists(subFolderToAdd))
        {
            throw new DirectoryNotFoundException();
        }
        
        var backupSettings = _backupSettingsReader.Read(folder);
        backupSettings.AddTrackedFolder(subFolderToAdd);
        _backupSettingsModifier.Overwrite(folder, backupSettings);
    }
}

public interface IBackupSettingsTrackFolder
{
    void Track(string subFolderToAdd, string folder);
}