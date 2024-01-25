using Backups.Infrastructure;
using Backups.Tests.UnitTests;
using FakeItEasy;

namespace Backups.Tests.IntegrationTests;

[TestFixture]
public class EnvironmentWrapperShould
{
    private const string ExpectedValue = "our variable";
    private readonly string _variableName = Guid.NewGuid().ToString();

    [OneTimeSetUp]
    public void SetUp()
    {
        Environment.SetEnvironmentVariable(_variableName, ExpectedValue);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable(_variableName, null);
    }
    
    [Test]
    public void Get_the_variable()
    {
        var returnedValue = new EnvironmentWrapper().GetVariable(_variableName);
        Assert.That(returnedValue, Is.EqualTo(ExpectedValue));
    }
    
    [Test]
    public void Throw_exception_if_variable_does_not_exist()
    {
        var environmentWrapper = new EnvironmentWrapper();
        Assert.Throws<EnvironmentVariableNotFoundException>(() => environmentWrapper.GetVariable(Guid.NewGuid().ToString()));
    }
}