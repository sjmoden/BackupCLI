using Backups.Actions.Track;
using Backups.Model;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class TrackShould
{
    private IBackupSettingsTrackFile _backupSettingsTrackFile = null!;
    private IBackupSettingsTrackFolder _backupSettingsTrackFolder = null!;

    [SetUp]
    public void SetUp()
    {
        _backupSettingsTrackFile = A.Fake<IBackupSettingsTrackFile>();
        _backupSettingsTrackFolder = A.Fake<IBackupSettingsTrackFolder>();
    }
    
    [Test]
    public void Track_the_file_in_the_directory()
    {
        var options = new TrackOptions
        {
            FileName = "test"
        };

        var trackFile = new Track(_backupSettingsTrackFile, _backupSettingsTrackFolder);
        trackFile.Execute(options);
        
        A.CallTo(() => _backupSettingsTrackFile.Track(options.FileName, Environment.CurrentDirectory)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _backupSettingsTrackFolder.Track(A<string>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Test]
    public void Track_the_folder_in_the_directory()
    {
        var options = new TrackOptions
        {
            FolderName = "test"
        };

        var trackFile = new Track(_backupSettingsTrackFile, _backupSettingsTrackFolder);
        trackFile.Execute(options);
        
        A.CallTo(() => _backupSettingsTrackFolder.Track(options.FolderName, Environment.CurrentDirectory)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _backupSettingsTrackFile.Track(A<string>._, A<string>._)).MustNotHaveHappened();
    }
}