using Microsoft.EntityFrameworkCore;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application;
using InnoShop.Infrastructure.ProductManagerAPI.Data;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Domain.Services;
using InnoShop.Infrastructure.ProductManagerAPI.Services;
using Npgsql;
using InnoShop.Domain.Enums;
using System.Text.Json.Serialization;

namespace InnoShop.Infrastructure.ProductManagerAPI;

public class Program {
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureSwaggerJwt();

        builder.Services.AddControllers().AddJsonOptions(options => {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        var config = new {
            database_host = builder.Configuration["Database:Host"] ?? "localhost",
            database_port = builder.Configuration["Database:Port"] ?? "5432",
            database_user = builder.Configuration.GetOrThrow("Database:User"),
            database_password = builder.Configuration.GetOrThrow("Database:Password"),
        };

        var dataSourceBuilder = new NpgsqlDataSourceBuilder($"Host={config.database_host};"
                            + $"Port={config.database_port};"
                            + $"Username={config.database_user};"
                            + $"Password={config.database_password};"
                            + $"Database=innoshop_products;");

        dataSourceBuilder.MapEnum<AviabilityStatus>();
        var dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dataSource));


        builder.Services.AddApplicationServices<IProductCommandHandler>(builder.Configuration);

        builder.Services.AddScoped<IProductDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<IProductFactory, ProductFactory>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.AddApplicationLayers();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}