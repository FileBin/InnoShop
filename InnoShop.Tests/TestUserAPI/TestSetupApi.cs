using InnoShop.Application.Shared.Auth;
using InnoShop.Infrastructure.UserManagerAPI;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Data.Common;

namespace InnoShop.Tests.TestUserAPI;

public class TestSetupApi {
    WebApplicationFactory<Program> webApp;
    HttpClient client;


    [SetUp]
    public virtual Task SetupAsync() {
        webApp = new TestWebApplicationFactory<Program>();
        client = webApp.CreateClient();
        return Task.CompletedTask;
    }

    [TearDown]
    public virtual void Cleanup() {
        client.Dispose();
        webApp.Dispose();
    }

    protected async Task<HttpResponseMessage> RegisterUser(UserCredentials credentials) {
        return await client.PostAsJsonAsync("api/accounts/register", new RegisterDto {
            Email = credentials.Email,
            Username = credentials.Username,
            Password = credentials.Password,
        });
    }

    protected async Task<HttpResponseMessage> LoginUser(UserCredentials credentials) {
        return await client.PostAsJsonAsync("api/accounts/login", new LoginDto {
            Login = credentials.Username,
            Password = credentials.Password,
        });
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