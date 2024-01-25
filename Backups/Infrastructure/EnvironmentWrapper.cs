namespace Backups.Infrastructure;

public class EnvironmentWrapper : IEnvironmentWrapper
{
    public string GetVariable(string variableName) => Environment.GetEnvironmentVariable(variableName) ?? throw new EnvironmentVariableNotFoundException();
}

public interface IEnvironmentWrapper
{
    string GetVariable(string variableName);
}