using InnoShop.Domain;
using InnoShop.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application;
using InnoShop.Infrastructure.ProductManagerAPI.Data;
using InnoShop.Application.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InnoShop.Infrastructure.ProductManagerAPI;

public class Program {
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplicationServices();

        builder.Services.ConfigureSwaggerJwt();

        builder.Services.AddControllers();

        var config = new {
            database_host = builder.Configuration["Database:Host"] ?? "localhost",
            database_port = builder.Configuration["Database:Port"] ?? "5432",
            database_user = builder.Configuration.GetOrThrow("Database:User"),
            database_password = builder.Configuration.GetOrThrow("Database:Password"),
            jwtIssuer = builder.Configuration.GetOrThrow("JwtIssuer"),
            jwtAudience = builder.Configuration.GetOrThrow("JwtAudience"),
            jwtSecurityKey = builder.Configuration.GetSecurityKey(),
        };

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql($"Host={config.database_host};"
                            + $"Port={config.database_port};"
                            + $"Username={config.database_user};"
                            + $"Password={config.database_password};"
                            + $"Database=innoshop_products;"));

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(options => {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.jwtIssuer,
                ValidAudience = config.jwtAudience,
                IssuerSigningKey = config.jwtSecurityKey,
            };
        });

        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(typeof(IUserCommandHandler).IsAssignableFrom)
            .Select(type => builder.Services.SingleOrDefault(desc => desc.ImplementationType == type))
            .Where(x => x != null)
            .ToList()
            .ForEach(desc => builder.Services.Remove(desc!));

        var app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.AddInnoshopApplicationMiddleware();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}