using System.IO.Abstractions;

namespace Backups.Infrastructure;

public class BackupSettingFilePath
{
    private readonly IFileSystem _fileSystem;

    public BackupSettingFilePath(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public const string BackupSettingsFileName = ".backupSettings";

    public static string Build(string directory) => Path.Combine(directory, BackupSettingsFileName);

    public void ThrowExceptionIfSettingsFilesDoesNotExist(string directory)
    {
        var filePath = Build(directory);

        if (!_fileSystem.File.Exists(filePath))
        {
            throw new BackupSettingsNotFoundException();
        }
    }
}