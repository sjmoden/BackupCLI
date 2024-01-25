using Azure;
using Azure.Security.KeyVault.Secrets;

namespace Backups.Actions.Archive;

public class KeyVault : IKeyVault
{
    private readonly ISecretClientFactory _secretClientFactory;

    public KeyVault(ISecretClientFactory secretClientFactory)
    {
        _secretClientFactory = secretClientFactory;
    }

    public bool TryGetSecret(string fileName, out string secret)
    {
        try
        {
            var response = _secretClientFactory.Create().GetSecret(fileName);
            secret =  response.Value.Value;
            return true;
        }
        catch (RequestFailedException e) when (e is { ErrorCode: "SecretNotFound", Status: 404 }) 
        {
            secret = string.Empty;
            return false;
        }
    }

    public void SaveSecret(string fileName, string password) => _secretClientFactory.Create().SetSecret(new KeyVaultSecret(fileName, password));
}

public interface IKeyVault
{
    bool TryGetSecret(string fileName, out string secret);
    void SaveSecret(string fileName, string password);
}