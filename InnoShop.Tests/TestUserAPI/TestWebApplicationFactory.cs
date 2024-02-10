using System.Data.Common;
using InnoShop.Domain.Services;
using Microsoft.AspNetCore.Hosting;

namespace InnoShop.Tests.TestUserAPI;
public class TestWebApplicationFactory<TProgram, TContext>
    : WebApplicationFactory<TProgram> where TProgram : class where TContext : DbContext {
    public required string DbName { get; init; }

    public bool UseSqlite = false;

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor is not null)
                services.Remove(dbConnectionDescriptor);

            if (UseSqlite) {
                services.AddDbContext<TContext>(options => {
                    options.UseSqlite($"Data Source={DbName}.db");
                });
            } else {
                services.AddDbContext<TContext>(options => {
                    options.UseInMemoryDatabase($"{DbName}");
                });
            }

            var mailService = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConfirmationMailService));
            if (mailService is not null)
                services.Remove(mailService);

            services.AddScoped<IConfirmationMailService, TestMailService>();
            
            if (UseSqlite) {
                MigrateDbContext(services);
            }
        });
    }

    public void MigrateDbContext(IServiceCollection serviceCollection) {
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