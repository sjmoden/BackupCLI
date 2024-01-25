using System.IO.Abstractions;
using System.Text.Json;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Actions;

public class BackupSettingsReader : IBackupSettingsReader
{
    private readonly IFileSystem _fileSystem;
    private readonly BackupSettingFilePath _backupSettingFilePath;
    
    public BackupSettingsReader(IFileSystem fileSystem, BackupSettingFilePath backupSettingFilePath)
    {
        _fileSystem = fileSystem;
        _backupSettingFilePath = backupSettingFilePath;
    }

    public BackupSettings Read(string directory)
    {
        var filePath = BackupSettingFilePath.Build(directory);
        _backupSettingFilePath.ThrowExceptionIfSettingsFilesDoesNotExist(directory);

        var contents = _fileSystem.File.ReadAllText(filePath);
        
        if (string.IsNullOrWhiteSpace(contents))
        {
            return new BackupSettings();
        }
        
        try
        {
            return JsonSerializer.Deserialize<BackupSettings>(contents)!;
        }
        catch (JsonException)
        {
            throw new BackupSettingsDeserialiseException();
        }
    }
}

public interface IBackupSettingsReader
{
    BackupSettings Read(string directory);
}