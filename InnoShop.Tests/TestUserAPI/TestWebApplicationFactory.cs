using System.Data.Common;
using InnoShop.Domain.Services;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using Microsoft.AspNetCore.Hosting;

namespace InnoShop.Tests.TestUserAPI;
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