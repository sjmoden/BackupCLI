using Backups.Actions.Archive;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class KeyVaultPasswordRetrieverShould
{
    private IKeyVault _keyVault = null!;
    private IPasswordGenerator _passwordGenerator = null!;
    private const string ExpectedPassword = "Password";
    private const string FileName = "filename";

    [SetUp]
    public void SetUp()
    {
        _keyVault = A.Fake<IKeyVault>();
        _passwordGenerator = A.Fake<IPasswordGenerator>();
    }

    [Test]
    public void Return_a_new_password_when_an_entry_in_key_vault_does_not_exist()
    {
        string ignored;
        A.CallTo(() => _keyVault.TryGetSecret(FileName, out ignored)).Returns(false);
        
        A.CallTo(() => _passwordGenerator.Generate()).Returns(ExpectedPassword);
        
        var keyVaultPassword = new KeyVaultPasswordRetriever(_keyVault, _passwordGenerator);
        var returnedPassword = keyVaultPassword.GetOrCreateANewPassword(FileName);
        
        Assert.That(returnedPassword, Is.EqualTo(ExpectedPassword));
    }
    
    [Test]
    public void Save_a_password_when_entry_does_not_exists_in_key_vault()
    {
        string ignored;
        A.CallTo(() => _keyVault.TryGetSecret(FileName, out ignored)).Returns(false);
        
        A.CallTo(() => _passwordGenerator.Generate()).Returns(ExpectedPassword);
        
        var keyVaultPassword = new KeyVaultPasswordRetriever(_keyVault, _passwordGenerator);
        var returnedPassword = keyVaultPassword.GetOrCreateANewPassword(FileName);
        
        A.CallTo(() => _keyVault.SaveSecret(FileName, ExpectedPassword)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Not_save_a_password_when_entry_already_exists_in_key_vault()
    {
        string ignored;
        A.CallTo(() => _keyVault.TryGetSecret(FileName, out ignored)).Returns(true)
            .AssignsOutAndRefParameters(ExpectedPassword);
        
        var keyVaultPassword = new KeyVaultPasswordRetriever(_keyVault, _passwordGenerator);
        var returnedPassword = keyVaultPassword.GetOrCreateANewPassword(FileName);
        
        A.CallTo(() => _keyVault.SaveSecret(A<string>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void Return_a_password_saved_in_key_vault()
    {
        string ignored;
        A.CallTo(() => _keyVault.TryGetSecret(FileName, out ignored)).Returns(true)
            .AssignsOutAndRefParameters(ExpectedPassword);
        
        var keyVaultPassword = new KeyVaultPasswordRetriever(_keyVault, _passwordGenerator);
        var returnedPassword = keyVaultPassword.GetOrCreateANewPassword(FileName);
        
        Assert.That(returnedPassword, Is.EqualTo(ExpectedPassword));
    }
}