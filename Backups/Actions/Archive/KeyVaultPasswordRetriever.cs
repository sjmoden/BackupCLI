namespace Backups.Actions.Archive;

public class KeyVaultPasswordRetriever: IKeyVaultPasswordRetriever
{
    private readonly IKeyVault _keyVault;
    private readonly IPasswordGenerator _passwordGenerator;

    public KeyVaultPasswordRetriever(IKeyVault keyVault, IPasswordGenerator passwordGenerator)
    {
        _keyVault = keyVault;
        _passwordGenerator = passwordGenerator;
    }

    public string GetOrCreateANewPassword(string fileName)
    {
        if (_keyVault.TryGetSecret(fileName, out var secret))
        {
            return secret;
        }

        var password = _passwordGenerator.Generate();
        _keyVault.SaveSecret(fileName, password);
        
        return password;
    }
}

public interface IKeyVaultPasswordRetriever
{
    string GetOrCreateANewPassword(string fileName);
}