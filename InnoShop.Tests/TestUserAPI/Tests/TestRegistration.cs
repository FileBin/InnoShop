namespace InnoShop.Tests.TestUserAPI;

[TestFixture]
public class TestRegistration : TestSetupApi {
    static readonly string TestEmail = "waffle@example.com";

    [Test]
    public async Task ValidParamenters() {
        var userCredentials = new UserCredentials {
            Email = TestEmail,
            Username = "onionWaffle",
            Password = TestPasswordGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    public async Task EmptyUsername() {
        var userCredentials = new UserCredentials {
            Email = TestEmail,
            Username = "",
            Password = TestPasswordGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);

        Assert.That(!result.IsSuccessStatusCode);
    }

    [Test]
    public async Task ShortPassword() {
        var userCredentials = new UserCredentials {
            Email = TestEmail,
            Username = "onionWaffle",
            Password = "abc"
        };

        var result = await RegisterUser(userCredentials);

        Assert.That(!result.IsSuccessStatusCode);
    }
}
