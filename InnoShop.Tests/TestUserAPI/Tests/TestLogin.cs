using InnoShop.Application.Shared.Models.Auth;

namespace InnoShop.Tests.TestUserAPI.Tests;

[TestFixture]
public class TestLogin : SetupUserApi {
    UserCredentials userCredentials;

    [SetUp]
    public async override Task SetupAsync() {
        await base.SetupAsync();

        userCredentials = new UserCredentials {
            Email = "waffle@example.com",
            Username = "waffle",
            Password = TestGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    [CancelAfter(10_000)]
    public async Task ValidParameters(CancellationToken cancellationToken) {
        await VerifyEmail(cancellationToken);
        var result = await LoginUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    [CancelAfter(10_000)]
    public async Task RefreshToken(CancellationToken cancellationToken) {
        await VerifyEmail(cancellationToken);
        var result = await LoginUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);

        var loginResult = await GetJsonContent<LoginResultDto>(result);

        result = await RefreshToken(loginResult.RefreshToken);
        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    public async Task WrongPassword() {
        userCredentials.Password = "abc123";
        var result = await LoginUser(userCredentials);
        Assert.That(!result.IsSuccessStatusCode);
    }
}
