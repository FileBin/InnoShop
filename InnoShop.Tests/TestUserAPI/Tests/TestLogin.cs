using System.Net.Mail;
using System.Text.RegularExpressions;

namespace InnoShop.Tests.TestUserAPI;

[TestFixture]
public class TestLogin : TestSetupApi {
    UserCredentials userCredentials;

    [SetUp]
    public async override Task SetupAsync() {
        await base.SetupAsync();

        userCredentials = new UserCredentials {
            Email = "waffle@example.com",
            Username = "waffle",
            Password = TestPasswordGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    [CancelAfter(10_000)]
    public async Task ValidParamenters(CancellationToken cancellationToken) {
        await VerifyEmail(cancellationToken);
        var result = await LoginUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    public async Task WrongPassword() {
        userCredentials.Password = "abc123";
        var result = await LoginUser(userCredentials);
        Assert.That(!result.IsSuccessStatusCode);
    }
}
