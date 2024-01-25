using PasswordGenerator;

namespace Backups.Actions.Archive;

public class PasswordGenerator : IPasswordGenerator
{
    private readonly IPassword _password;

    public PasswordGenerator(IPassword password) => _password = password;

    public string Generate()
    {
        _password.LengthRequired(32);
        _password.IncludeLowercase();
        _password.IncludeUppercase();
        _password.IncludeNumeric();
        _password.IncludeSpecial();
        return _password.Next();
    }
}

public interface IPasswordGenerator
{
    string Generate();
}