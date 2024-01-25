using System.IO.Abstractions;
using System.Text;
using Backups.Infrastructure;

namespace Backups.Actions.Evaluate;

public class EvaluateFolder : IEvaluateFolder
{
    private readonly IFileSystem _mockFileSystem;
    private readonly BackupSettingFilePath _backupSettingFilePath;
    private readonly IBackupSettingsReader _backupSettingsReader;

    public EvaluateFolder(IFileSystem mockFileSystem, BackupSettingFilePath backupSettingFilePath,
        IBackupSettingsReader backupSettingsReader)
    {
        _mockFileSystem = mockFileSystem;
        _backupSettingFilePath = backupSettingFilePath;
        _backupSettingsReader = backupSettingsReader;
    }

    public EvaluationResult Evaluate(string folderRoot)
    {
        _backupSettingFilePath.ThrowExceptionIfSettingsFilesDoesNotExist(folderRoot);

        var subDirectories = _mockFileSystem.Directory.GetDirectories(folderRoot)
            .Select(f => f[(folderRoot.Length + 1)..]);

        var files = _mockFileSystem.Directory
            .GetFiles(folderRoot, "*")
            .Select(f => f[(folderRoot.Length + 1)..])
            .Where(f => f != BackupSettingFilePath.BackupSettingsFileName);
        
        var backupSettings = _backupSettingsReader.Read(folderRoot);
        
        var trackedFiles = backupSettings.TrackedFiles.ToList();
        var untrackedFiles = files.Where(f => !backupSettings.DoesFileExistInTrackedFilesList(f))
            .ToList();

        var trackedFolders = backupSettings.TrackedFolders.ToList();
        var untrackedFolders = subDirectories.Where(d => !backupSettings.DoesFolderExistInTrackedFolderList(d))
            .ToList();

        return new EvaluationResult(untrackedFiles, trackedFiles, untrackedFolders, trackedFolders);
    }
}

public class EvaluationResult
{
    public EvaluationResult(List<string> untrackedFiles, List<string> trackedFiles, List<string> untrackedFolders, List<string> trackedFolders)
    {
        UntrackedFiles = untrackedFiles;
        TrackedFiles = trackedFiles;
        UntrackedFolders = untrackedFolders;
        TrackedFolders = trackedFolders;
    }

    public List<string> UntrackedFiles { get; }
    public List<string> UntrackedFolders { get; }
    public List<string> TrackedFiles { get; }
    public List<string> TrackedFolders { get; }

    public string GetEvaluationSummary()
    {
        var stringBuilder = new StringBuilder();
        if(UntrackedFiles.Any())
        {
            stringBuilder.AppendLine("Untracked Files:");
            UntrackedFiles.ForEach(f => stringBuilder.AppendLine($"    {f}"));
            stringBuilder.AppendLine();
        }
        
        if(TrackedFiles.Any())
        {
            stringBuilder.AppendLine("Tracked Files:");
            TrackedFiles.ForEach(f => stringBuilder.AppendLine($"    {f}"));
            stringBuilder.AppendLine();
        }
        
        if(UntrackedFolders.Any())
        {
            stringBuilder.AppendLine("Untracked Folders:");
            UntrackedFolders.ForEach(f => stringBuilder.AppendLine($"    {f}"));
            stringBuilder.AppendLine();
        }
        
        if(TrackedFolders.Any())
        {
            stringBuilder.AppendLine("Tracked Folders:");
            TrackedFolders.ForEach(f => stringBuilder.AppendLine($"    {f}"));
            stringBuilder.AppendLine();
        }
        
        return stringBuilder.ToString();
    }
}

public interface IEvaluateFolder
{
    EvaluationResult Evaluate(string folderRoot);
}