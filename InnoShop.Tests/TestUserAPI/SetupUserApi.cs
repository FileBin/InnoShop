using InnoShop.Infrastructure.UserManagerAPI;
using System.Net.Http.Json;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mail;
using InnoShop.Application.Shared.Models.Auth;
using System.Net;

namespace InnoShop.Tests.TestUserAPI;

public class SetupUserApi {
    WebApplicationFactory<Program> webAppUserApi;
    protected HttpClient clientUserApi;

    [SetUp]
    public virtual Task SetupAsync() {
        webAppUserApi =
        new TestWebApplicationFactory<Program, ApplicationDbContext>() {
            UseSqlite = true,
            DbName = "UserDb",
        };
        clientUserApi = webAppUserApi.CreateClient();
        return Task.CompletedTask;
    }

    [TearDown]
    public virtual void Cleanup() {
        TestMailService.ClearStorage();
        clientUserApi.Dispose();
        webAppUserApi.Dispose();
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
        Assert.That(result.IsSuccessStatusCode || result.StatusCode == HttpStatusCode.NotFound);
    }

    protected async Task<HttpResponseMessage> RegisterUser(UserCredentials credentials) {
        return await clientUserApi.PostAsJsonAsync("/api/accounts/register", new RegisterDto(
            email: credentials.Email,
            username: credentials.Username,
            password: credentials.Password
        ));
    }

    protected async Task<HttpResponseMessage> LoginUser(UserCredentials credentials) {
        return await clientUserApi.PostAsJsonAsync("/api/accounts/login", new LoginDto(
            login: credentials.Username,
            password: credentials.Password
        ));
    }

        protected async Task<HttpResponseMessage> RefreshToken(string? jwtToken = null) {
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/accounts/refresh_token")) {
            if (jwtToken is not null) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtToken);
            }
            return await clientUserApi.SendAsync(requestMessage);
        }
    }

    protected async Task<HttpResponseMessage> GetUserInfo(string? jwtToken = null) {
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/accounts/info")) {
            if (jwtToken is not null) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtToken);
            }
            return await clientUserApi.SendAsync(requestMessage);
        }
    }

    protected async Task<HttpResponseMessage> FollowLink(string link) {
        return await clientUserApi.GetAsync(link);
    }

    protected async Task<T> GetJsonContent<T>(HttpResponseMessage response) {
        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(jsonString);
        if (result is not null)
            return result;

        throw new JsonSerializationException($"Cannot convert {jsonString} to type {typeof(T).FullName}");
    }
}
