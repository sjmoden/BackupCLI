using System.IO.Abstractions;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Actions.Archive;

public interface ITempArchiveZip
{
    string ZipFileToTempFolder(BackupSettings backupSettings, string password);
    void DeleteTempZip(string filePath);
}

public class TempArchiveZip : ITempArchiveZip
{
    private readonly IGuidCreator _guidCreator;
    private readonly ISevenZip _sevenZip;
    private readonly IFileSystem _fileSystem;

    public TempArchiveZip(IGuidCreator guidCreator, ISevenZip sevenZip, IFileSystem fileSystem)
    {
        _guidCreator = guidCreator;
        _sevenZip = sevenZip;
        _fileSystem = fileSystem;
    }

    public string ZipFileToTempFolder(BackupSettings backupSettings, string password)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{backupSettings.FileName}_{DateTime.Now:yyyyMMddHHss}_{_guidCreator.NewGuid()}.7z");
        backupSettings.GetTrackedFilesAndFoldersThrowIfNotFound().ToList().ForEach(trackedFile =>
        {
            _sevenZip.AddFileToZip(new AddFileToZipParameters(password, tempFilePath, trackedFile));
        });
        
        return tempFilePath;
    }

    public void DeleteTempZip(string filePath)
    {
        _fileSystem.File.Delete(filePath);
    }
}