namespace Backups.Infrastructure;

public class ConsoleWrapper : IConsoleWrapper
{
    public void WriteLine(string line) => Console.WriteLine(line);
}

public interface IConsoleWrapper
{
    void WriteLine(string line);
}