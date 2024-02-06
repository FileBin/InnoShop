using InnoShop.Application.Shared.Auth;
using InnoShop.Infrastructure.UserManagerAPI;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mail;

namespace InnoShop.Tests.TestUserAPI;

public class TestSetupApi {
    WebApplicationFactory<Program> webApp;
    protected HttpClient client;

    [SetUp]
    public virtual Task SetupAsync() {
        webApp = new TestWebApplicationFactory<Program>();
        client = webApp.CreateClient();
        return Task.CompletedTask;
    }

    [TearDown]
    public virtual void Cleanup() {
        TestMailService.ClearStorage();
        client.Dispose();
        webApp.Dispose();
    }

    protected async Task VerifyEmail(CancellationToken token) {
        MailMessage? mail = null;
        while (mail == null) {
            if (TestMailService.PeekMailFromHeap(out mail))
                break;
            Thread.Sleep(100);

            if (token.IsCancellationRequested) Assert.Fail("Test ran out of time");
        }
        Assert.That(mail, Is.Not.Null);

        var result = await FollowLink(mail.Body);
        Assert.That(result.IsSuccessStatusCode);
    }

    protected async Task<HttpResponseMessage> RegisterUser(UserCredentials credentials) {
        return await client.PostAsJsonAsync("/api/accounts/register", new RegisterDto {
            Email = credentials.Email,
            Username = credentials.Username,
            Password = credentials.Password,
        });
    }

    protected async Task<HttpResponseMessage> LoginUser(UserCredentials credentials) {
        return await client.PostAsJsonAsync("/api/accounts/login", new LoginDto {
            Login = credentials.Username,
            Password = credentials.Password,
        });
    }

    protected async Task<HttpResponseMessage> GetUserInfo(string? jwtToken = null) {
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/accounts/info")) {
            if (jwtToken is not null) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtToken);
            }
            return await client.SendAsync(requestMessage);
        }
    }

    protected async Task<HttpResponseMessage> FollowLink(string link) {
        return await client.GetAsync(link);
    }

    protected async Task<T> GetJsonContent<T>(HttpResponseMessage response) {
        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(jsonString);
        if (result is not null)
            return result;

        throw new JsonSerializationException($"Cannot convert {jsonString} to type {typeof(T).FullName}");
    }
}
