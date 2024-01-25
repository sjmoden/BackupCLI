using System.IO.Abstractions;
using System.Text.Json;
using Backups.Infrastructure;

namespace Backups.Model;

public record FileHash(string FileName, string Hash);

public class BackupSettings
{
    public string FileName { get; init; }
    public string AzureStorageAccountName { get; init; }
    public string Container { get; init; }
    public IList<string> TrackedFiles { get; init; } = new List<string>();
    public IList<string> TrackedFolders { get; init; } = new List<string>();
    public IList<FileHash> PreviousHashesOfFiles { get; init; } = new List<FileHash>();

    public void AddTrackedFile(string fileName)
    {
        if (TrackedFiles.Contains(fileName))
        {
            return;
        }
        
        TrackedFiles.Add(fileName);
    }

    public IEnumerable<string> GetTrackedFilesAndFoldersThrowIfNotFound()
    {
        if (!TrackedFiles.Any() && !TrackedFolders.Any())
        {
            throw new NoTrackedFilesOrFoldersException();            
        }
        
        return TrackedFiles.Union(TrackedFolders);
    }

    protected bool Equals(BackupSettings other) =>
        FileName == other.FileName 
        && AzureStorageAccountName == other.AzureStorageAccountName
        && Container == other.Container
        && TrackedFolders.SequenceEqual(other.TrackedFolders)
        && TrackedFiles.SequenceEqual(other.TrackedFiles);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BackupSettings)obj);
    }

    public override int GetHashCode() => HashCode.Combine(FileName, AzureStorageAccountName, TrackedFiles, Container, TrackedFolders);

    public bool DoesFileExistInTrackedFilesList(string fileName) => TrackedFiles.Contains(fileName);

    public string ToSerialisedString() => JsonSerializer.Serialize(this);

    public virtual bool HaveFilesChangedSinceLastUploadAndUpdateRegister() =>
        HaveFilesChangedSinceLastUploadAndUpdateRegister(new FileSystem());

    public bool HaveFilesChangedSinceLastUploadAndUpdateRegister(IFileSystem fileSystem)
    {
        var result = false;
        var checkSum = new CheckSum(fileSystem);

        foreach (var trackedFolder in TrackedFolders)
        {
            var files = fileSystem.Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, trackedFolder));

            foreach (var file in files)
            {
                var hash = checkSum.Get(Path.Combine(Environment.CurrentDirectory, file));
                var fileName = Path.Combine(trackedFolder, Path.GetFileName(file));
                var fileHash = new FileHash(fileName,hash);
                
                var matchedEntry = PreviousHashesOfFiles.SingleOrDefault(p => p.FileName.Equals(fileName));

                if (matchedEntry == fileHash)
                {
                    continue;
                }
                
                result = true;
            
                if (matchedEntry == null)
                {
                    PreviousHashesOfFiles.Add(fileHash);
                    continue;
                }

                PreviousHashesOfFiles.Remove(matchedEntry);
                PreviousHashesOfFiles.Add(fileHash);
            }
        }
        
        
        foreach (var trackedFile in TrackedFiles)
        {
            var hash = checkSum.Get(Path.Combine(Environment.CurrentDirectory, trackedFile));
            var fileHash = new FileHash(trackedFile,hash);

            var matchedEntry = PreviousHashesOfFiles.SingleOrDefault(p => p.FileName.Equals(trackedFile));
            
            if (matchedEntry == fileHash)
            {
                continue;
            }
            
            result = true;
            
            if (matchedEntry == null)
            {
                PreviousHashesOfFiles.Add(fileHash);
                continue;
            }

            PreviousHashesOfFiles.Remove(matchedEntry);
            PreviousHashesOfFiles.Add(fileHash);
        }
        
        return result;
    }

    public void AddTrackedFolder(string folderName)
    {
        if (TrackedFolders.Contains(folderName))
        {
            return;
        }
        
        TrackedFolders.Add(folderName);
    }

    public bool DoesFolderExistInTrackedFolderList(string folder) => TrackedFolders.Contains(folder);
}