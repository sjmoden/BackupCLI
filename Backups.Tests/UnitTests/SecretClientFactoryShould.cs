using Backups.Actions.Archive;
using Backups.Infrastructure;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class SecretClientFactoryShould
{
    [Test]
    public void Create_a_secret_client()
    {
        const string vaultName = "456789";
        const string variableName = "BackupsVaultName";
        
        var environmentWrapper = A.Fake<IEnvironmentWrapper>();
        A.CallTo(() => environmentWrapper.GetVariable(variableName)).Returns(vaultName);
        
        var secretClient = new SecretClientFactory(environmentWrapper).Create();
        Assert.That(secretClient.VaultUri.AbsoluteUri, Is.EqualTo($"https://{vaultName}.vault.azure.net/"));
    }
}

