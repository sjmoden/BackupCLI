using Backups.Model;

namespace Backups.Actions.Track;

public class Track
{
    private readonly IBackupSettingsTrackFile _backupSettingsTrackFile;
    private readonly IBackupSettingsTrackFolder _backupSettingsTrackFolder;

    public Track(IBackupSettingsTrackFile backupSettingsTrackFile, IBackupSettingsTrackFolder backupSettingsTrackFolder)
    {
        _backupSettingsTrackFile = backupSettingsTrackFile;
        _backupSettingsTrackFolder = backupSettingsTrackFolder;
    }

    public void Execute(TrackOptions trackOptions)
    {
        if (!string.IsNullOrWhiteSpace(trackOptions.FileName))
        {
            _backupSettingsTrackFile.Track(trackOptions.FileName, Environment.CurrentDirectory);
            return;
        }
        _backupSettingsTrackFolder.Track(trackOptions.FolderName, Environment.CurrentDirectory);
    }
}