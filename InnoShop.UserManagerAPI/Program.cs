using InnoShop.Domain;
using InnoShop.Domain.Services;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using InnoShop.Infrastructure.UserManagerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application;
using InnoShop.Application.Shared.Interfaces;

namespace InnoShop.Infrastructure.UserManagerAPI;

public class Program {
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplicationServices<IUserCommandHandler>();

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
                            + $"Database=innoshop_users;"));

        builder.Services.AddDefaultIdentity<ShopUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.Configure<IdentityOptions>(options => {
            options.SignIn.RequireConfirmedEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;
        });

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

        builder.Services.AddScoped<IConfirmationMailService, MailService>();
        builder.Services.AddScoped<IPasswordResetMailService, MailService>();

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