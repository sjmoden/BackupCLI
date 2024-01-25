using FakeItEasy;
using PasswordGenerator;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class PasswordGeneratorShould
{
    [Test]
    public void Generate_a_password()
    {
        var password = A.Fake<IPassword>();

        var passwordGenerator = new Actions.Archive.PasswordGenerator(password);
        passwordGenerator.Generate();
        
        A.CallTo(() => password.LengthRequired(32)).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => password.IncludeLowercase()).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => password.IncludeUppercase()).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => password.IncludeNumeric()).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => password.IncludeSpecial()).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => password.Next()).MustHaveHappenedOnceExactly());
    }
}