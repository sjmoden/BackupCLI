using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Backups.Infrastructure;

namespace Backups.Actions.Archive;

public class SecretClientFactory : ISecretClientFactory 
{
    private const string VariableName = "BackupsVaultName";
    private readonly IEnvironmentWrapper _environmentWrapper;

    public SecretClientFactory(IEnvironmentWrapper environmentWrapper) => _environmentWrapper = environmentWrapper;

    public SecretClient Create() => new(new Uri($"https://{_environmentWrapper.GetVariable(VariableName)}.vault.azure.net/"), new AzureCliCredential());
}

public interface ISecretClientFactory
{
    SecretClient Create();
}