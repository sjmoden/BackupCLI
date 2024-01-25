using System.IO.Abstractions;

namespace Backups.Actions.Track;

public class BackupSettingsTrackFile: IBackupSettingsTrackFile
{
    private readonly IFileSystem _fileSystem;
    private readonly IBackupSettingsReader _backupSettingsReader;
    private readonly IBackupSettingsModifier _backupSettingsModifier;
    public BackupSettingsTrackFile(IFileSystem fileSystem, IBackupSettingsReader backupSettingsReader, IBackupSettingsModifier backupSettingsModifier)
    {
        _fileSystem = fileSystem;
        _backupSettingsReader = backupSettingsReader;
        _backupSettingsModifier = backupSettingsModifier;
    }

    public void Track(string file, string folder)
    {
        if (!_fileSystem.File.Exists(file))
        {
            throw new FileNotFoundException();
        }

        var backupSettings = _backupSettingsReader.Read(folder);
        backupSettings.AddTrackedFile(file);
        _backupSettingsModifier.Overwrite(folder, backupSettings);
    }
}

public interface IBackupSettingsTrackFile
{
    void Track(string file, string folder);
}