using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Tests.TestUserAPI;

namespace InnoShop.Tests.TestProductAPI.Tests;

class SetupProductAuth : SetupProductApi {


    public override async Task SetupAsync() {
        await base.SetupAsync();

        var userCredentials = new UserCredentials {
            Email = "waffle@example.com",
            Username = "waffle",
            Password = TestGenerator.GenerateRandomPassword()
        };

        var result = await RegisterUser(userCredentials);
        Assert.That(result.IsSuccessStatusCode);

        using (var canceller = new CancellationTokenSource(10_000)) {
            await VerifyEmail(canceller.Token);
        }

        result = await LoginUser(userCredentials);
        
        Assert.That(result.IsSuccessStatusCode);

        var loginResult = await GetJsonContent<LoginResultDto>(result);
        
        SetJwtToken(loginResult.AccessToken);
    }
}