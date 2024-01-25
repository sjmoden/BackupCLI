using System.IO.Abstractions;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Actions;

public class BackupSettingsModifier : IBackupSettingsModifier
{
    private readonly IFileSystem _fileSystem;

    public BackupSettingsModifier(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool TryCreate(string path, BackupSettings backUpSettings)
    {
        var filePath = BackupSettingFilePath.Build(path);
        
        if (_fileSystem.File.Exists(filePath))
        {
            return false;
        }
        
        Overwrite(path, backUpSettings);
        return true;
    }

    public void Overwrite(string path, BackupSettings backUpSettings)
    {
        var filePath = BackupSettingFilePath.Build(path);
        _fileSystem.File.WriteAllText(filePath, backUpSettings.ToSerialisedString());
    }
}

public interface IBackupSettingsModifier
{
    bool TryCreate(string path, BackupSettings backUpSettings);
    void Overwrite(string path, BackupSettings backUpSettings);
}