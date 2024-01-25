using Azure;
using Azure.Security.KeyVault.Secrets;
using Backups.Actions.Archive;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class KeyVaultShould
{
    private ISecretClientFactory _secretClientFactory = null!;
    private SecretClient _secretClient = null!;
    private const string FileName = "file";
    private const string Password = "password";

    [SetUp]
    public void SetUp()
    {
        _secretClientFactory = A.Fake<ISecretClientFactory>();
        _secretClient = A.Fake<SecretClient>();
        A.CallTo(() => _secretClientFactory.Create()).Returns(_secretClient);
    }

    [Test]
    public void Return_false_and_empty_secret_when_no_secret_exists()
    {
        A.CallTo(() => _secretClient.GetSecret(FileName, A<string>._, A<CancellationToken>._))
            .Throws(new RequestFailedException(404, "message", "SecretNotFound", new Exception()));
        
        var returnValue = new KeyVault(_secretClientFactory).TryGetSecret(FileName, out var returnedSecret);
        
        Assert.Multiple(() =>
        {
            Assert.That(returnedSecret , Is.Empty);
            Assert.That(returnValue , Is.False);
        });
    }
    
    
    [Test]
    public void Return_true_and__secret_when_secret_exists()
    {
        var mockResponse = A.Fake<Response>();
        var keyVaultSecret = SecretModelFactory.KeyVaultSecret(new SecretProperties(FileName), Password);
        var response = Response.FromValue(keyVaultSecret, mockResponse);
        
        A.CallTo(() => _secretClient.GetSecret(FileName, A<string>._, A<CancellationToken>._))
            .Returns(response);
        
        var returnValue = new KeyVault(_secretClientFactory).TryGetSecret(FileName, out var returnedSecret);
        
        Assert.Multiple(() =>
        {
            Assert.That(returnValue , Is.True);
            Assert.That(returnedSecret , Is.EqualTo(Password));
        });
    }

    [Test]
    public void Save_secret()
    {
        new KeyVault(_secretClientFactory).SaveSecret(FileName, Password);
        
        A.CallTo(() =>
                _secretClient.SetSecret(
                    A<KeyVaultSecret>.That.Matches(k => k.Name.Equals(FileName) && k.Value.Equals(Password)),
                    A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}