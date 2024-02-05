using InnoShop.Application.Shared.Auth;
using InnoShop.Infrastructure.UserManagerAPI;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Data.Common;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using InnoShop.Domain.Services;
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

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class {
    public readonly static string ConnectionString = "Data Source=TestDb.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor is not null)
                services.Remove(dbConnectionDescriptor);

            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(ConnectionString);
            });

            var mailService = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConfirmationMailService));
            if (mailService is not null)
                services.Remove(mailService);

            services.AddScoped<IConfirmationMailService, TestMailService>();

            MigrateDbContext<ApplicationDbContext>(services);
        });
    }

    public void MigrateDbContext<TContext>(IServiceCollection serviceCollection) where TContext : DbContext {
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetService<TContext>();

        if (context?.Database.IsNpgsql() ?? true) {
            throw new Exception("Use Sqlite instead of sql server!");
        }

        context.Database.EnsureDeleted();

        context.Database.Migrate();

    }
}