using InnoShop.Application.Shared.Auth;
using InnoShop.Domain;
using InnoShop.Infrastructure.UserManagerAPI;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using InnoShop.Application.Shared;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Data.Common;

namespace InnoShop.Tests;

public class TestUserAPI {
    WebApplicationFactory<Program> webApp;
    HttpClient client;

    public static string RandomString(int length) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    [SetUp]
    public void SetupAsync() {
        webApp = new CustomWebApplicationFactory<Program>();

        client = webApp.CreateClient();
    }

    [TearDown]
    public void Cleanup() {
        client.Dispose();
        webApp.Dispose();
    }

    [Test]
    public async Task RegisterPositiveTest() {
        var userCredentials = new UserCredentials("waffle@example.com", "onionWaffle", "IL0veC4eese");

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    public async Task RegisterNegativeTestEmptyUsername() {
        var userCredentials = new UserCredentials("sus", "", "IL0veC4eese");

        var result = await RegisterUser(userCredentials);

        Assert.That(!result.IsSuccessStatusCode);
    }

    [Test]
    public async Task RegisterNegativeTestShortPassword() {
        var userCredentials = new UserCredentials("sus", "abc", "abc");

        var result = await RegisterUser(userCredentials);

        Assert.That(!result.IsSuccessStatusCode);
    }

    private async Task<HttpResponseMessage> RegisterUser(UserCredentials credentials) {
        return await client.PostAsJsonAsync("api/accounts/register", new RegisterDto {
            Email = credentials.email,
            Username = credentials.username,
            Password = credentials.password,
        });
    }
}

record UserCredentials(string email, string username, string password);

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));
            if (dbConnectionDescriptor is not null)
                services.Remove(dbConnectionDescriptor);

            services.AddDbContext<ApplicationDbContext>((container, options) => {
                options.UseInMemoryDatabase("InnoShopTest");
            });
        });
    }
}
