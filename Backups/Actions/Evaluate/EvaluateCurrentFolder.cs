using Backups.Infrastructure;

namespace Backups.Actions.Evaluate;

public class EvaluateCurrentFolder
{
    private readonly IConsoleWrapper _consoleWrapper;
    private readonly IEvaluateFolder _evaluateFolder;

    public EvaluateCurrentFolder(IConsoleWrapper consoleWrapper, IEvaluateFolder evaluateFolder)
    {
        _consoleWrapper = consoleWrapper;
        _evaluateFolder = evaluateFolder;
    }

    public void Evaluate()
    {
        var result = _evaluateFolder.Evaluate(Environment.CurrentDirectory);
        _consoleWrapper.WriteLine(result.GetEvaluationSummary());
    }
}