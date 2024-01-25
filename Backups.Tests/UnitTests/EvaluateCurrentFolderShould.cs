using Backups.Actions.Evaluate;
using Backups.Infrastructure;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class EvaluateCurrentFolderShould
{
    private readonly List<string> _unTrackedFiles = new()
    {
        UntrackedFile1, UntrackedFile2
    };
    
    private readonly List<string> _trackedFiles = new()
    {
        TrackedFile1, TrackedFile2
    };
    
    private readonly List<string> _unTrackedFolders = new()
    {
        UntrackedFolder1, UntrackedFolder2
    };
    
    private readonly List<string> _trackedFolders = new()
    {
        TrackedFolder1, TrackedFolder2
    };

    private const string UntrackedFile1 = "file1.jpg";
    private const string UntrackedFile2 = "file2.jpg";
    private const string UntrackedFolder1 = "Folder1";
    private const string UntrackedFolder2 = "Folder2";
    private const string TrackedFile1 = "tfile1.jpg";
    private const string TrackedFile2 = "tfile2.jpg";
    private const string TrackedFolder1 = "tFolder1";
    private const string TrackedFolder2 = "tFolder2";

    [Test]
    public void Log_only_untracked_files_to_console_files()
    {
        var fileString = $"Untracked Files:{Environment.NewLine}    {UntrackedFile1}{Environment.NewLine}    {UntrackedFile2}{Environment.NewLine}{Environment.NewLine}";
        var consoleWrapper = A.Fake<IConsoleWrapper>();
    
        var evaluateFolder = A.Fake<IEvaluateFolder>();
        A.CallTo(() => evaluateFolder.Evaluate(Environment.CurrentDirectory)).Returns(new EvaluationResult(_unTrackedFiles, new List<string>(), new List<string>(), new List<string>()));
            
        var evaluateCurrentFolder = new EvaluateCurrentFolder(consoleWrapper, evaluateFolder);
        evaluateCurrentFolder.Evaluate();

        A.CallTo(() => consoleWrapper.WriteLine(fileString)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Log_only_tracked_files_to_console_files()
    {
        var fileString = $"Tracked Files:{Environment.NewLine}    {TrackedFile1}{Environment.NewLine}    {TrackedFile2}{Environment.NewLine}{Environment.NewLine}";
        var consoleWrapper = A.Fake<IConsoleWrapper>();
    
        var evaluateFolder = A.Fake<IEvaluateFolder>();
        A.CallTo(() => evaluateFolder.Evaluate(Environment.CurrentDirectory)).Returns(new EvaluationResult(new List<string>(), _trackedFiles, new List<string>(), new List<string>()));
            
        var evaluateCurrentFolder = new EvaluateCurrentFolder(consoleWrapper, evaluateFolder);
        evaluateCurrentFolder.Evaluate();

        A.CallTo(() => consoleWrapper.WriteLine(fileString)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Log_only_tracked_folders_to_console_files()
    {
        var fileString = $"Tracked Folders:{Environment.NewLine}    {TrackedFolder1}{Environment.NewLine}    {TrackedFolder2}{Environment.NewLine}{Environment.NewLine}";
        var consoleWrapper = A.Fake<IConsoleWrapper>();
    
        var evaluateFolder = A.Fake<IEvaluateFolder>();
        A.CallTo(() => evaluateFolder.Evaluate(Environment.CurrentDirectory)).Returns(new EvaluationResult(new List<string>(), new List<string>(), new List<string>(), _trackedFolders));
            
        var evaluateCurrentFolder = new EvaluateCurrentFolder(consoleWrapper, evaluateFolder);
        evaluateCurrentFolder.Evaluate();

        A.CallTo(() => consoleWrapper.WriteLine(fileString)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Log_only_untracked_folders_to_console_files()
    {
        var fileString = $"Untracked Folders:{Environment.NewLine}    {UntrackedFolder1}{Environment.NewLine}    {UntrackedFolder2}{Environment.NewLine}{Environment.NewLine}";
        var consoleWrapper = A.Fake<IConsoleWrapper>();
    
        var evaluateFolder = A.Fake<IEvaluateFolder>();
        A.CallTo(() => evaluateFolder.Evaluate(Environment.CurrentDirectory)).Returns(new EvaluationResult(new List<string>(), new List<string>(), _unTrackedFolders, new List<string>()));
            
        var evaluateCurrentFolder = new EvaluateCurrentFolder(consoleWrapper, evaluateFolder);
        evaluateCurrentFolder.Evaluate();

        A.CallTo(() => consoleWrapper.WriteLine(fileString)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Log_everything_to_console_files()
    {
        var fileString = $"Untracked Files:{Environment.NewLine}    {UntrackedFile1}{Environment.NewLine}    {UntrackedFile2}{Environment.NewLine}{Environment.NewLine}Tracked Files:{Environment.NewLine}    {TrackedFile1}{Environment.NewLine}    {TrackedFile2}{Environment.NewLine}{Environment.NewLine}Untracked Folders:{Environment.NewLine}    {UntrackedFolder1}{Environment.NewLine}    {UntrackedFolder2}{Environment.NewLine}{Environment.NewLine}Tracked Folders:{Environment.NewLine}    {TrackedFolder1}{Environment.NewLine}    {TrackedFolder2}{Environment.NewLine}{Environment.NewLine}";
        var consoleWrapper = A.Fake<IConsoleWrapper>();
    
        var evaluateFolder = A.Fake<IEvaluateFolder>();
        A.CallTo(() => evaluateFolder.Evaluate(Environment.CurrentDirectory)).Returns(new EvaluationResult(_unTrackedFiles, _trackedFiles, _unTrackedFolders, _trackedFolders));
            
        var evaluateCurrentFolder = new EvaluateCurrentFolder(consoleWrapper, evaluateFolder);
        evaluateCurrentFolder.Evaluate();

        A.CallTo(() => consoleWrapper.WriteLine(fileString)).MustHaveHappenedOnceExactly();
    }
}