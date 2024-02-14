using InnoShop.Domain;
using InnoShop.Domain.Services;
using InnoShop.Infrastructure.UserManagerAPI.Data;
using InnoShop.Infrastructure.UserManagerAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Domain.Entities.Roles;

namespace InnoShop.Infrastructure.UserManagerAPI;

public class Program {
    private static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.ConfigureSwaggerJwt();

        builder.Services.AddControllers();

        var config = new {
            database_host = builder.Configuration["Database:Host"] ?? "localhost",
            database_port = builder.Configuration["Database:Port"] ?? "5432",
            database_user = builder.Configuration.GetOrThrow("Database:User"),
            database_password = builder.Configuration.GetOrThrow("Database:Password"),
        };

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql($"Host={config.database_host};"
                            + $"Port={config.database_port};"
                            + $"Username={config.database_user};"
                            + $"Password={config.database_password};"
                            + $"Database=innoshop_users;"));

        builder.Services
            .AddDefaultIdentity<ShopUser>()
            .AddRoles<ShopRole>()
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

        builder.Services.AddApplicationServices<IUserCommandHandler>(builder.Configuration);


        builder.Services.AddScoped<IConfirmationMailService, MailService>();
        builder.Services.AddScoped<IPasswordResetMailService, MailService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //FIXME: i don't have trusted certs so just fox example https will be disabled
        //app.UseHttpsRedirection();
        app.AddApplicationLayers();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        using (var scope = app.Services.CreateScope()) {
            await scope.ServiceProvider.ConfigureRolesAsync();
        }

        app.Run();
    }
}