using InnoShop.Application.Shared.Auth;

namespace InnoShop.Tests.TestUserAPI;

public class AuthTest : TestSetupApi {
    [Test]
    [CancelAfter(10_000)]
    public async Task UserInfoGet(CancellationToken cancellationToken) {
        var userCredentials = new UserCredentials {
            Email = "waffle@example.com",
            Username = "waffle",
            Password = TestPasswordGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);
        
        await VerifyEmail(cancellationToken);
        
        result = await LoginUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);

        var loginResult = await GetJsonContent<LoginResult>(result);
        Assert.That(loginResult.IsSuccessful);
        result = await GetUserInfo(loginResult.Token);
        Assert.That(result.IsSuccessStatusCode);

        var userInfo = await GetJsonContent<UserInfoDto>(result);

        Assert.That(userInfo.Username, Is.EqualTo(userCredentials.Username));
        Assert.That(userInfo.Email, Is.EqualTo(userCredentials.Email));
    }

    [Test]
    [CancelAfter(10_000)]
    public async Task UserInfoUnathorized(CancellationToken cancellationToken) {
        var userCredentials = new UserCredentials {
            Email = "waffle@example.com",
            Username = "waffle",
            Password = TestPasswordGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);

        result = await GetUserInfo();
        Assert.That(!result.IsSuccessStatusCode);
    }


}
