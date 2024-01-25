namespace Backups.Infrastructure;

public class GuidCreator : IGuidCreator
{
    public string NewGuid() => Guid.NewGuid().ToString();
}

public interface IGuidCreator
{
    string NewGuid();
}