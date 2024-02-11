using InnoShop.Domain.Entities.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.Domain;

public static class ConfigureServices {
    public static async Task<IServiceProvider> ConfigureRolesAsync(this IServiceProvider services) {
        var userManager = services.GetRequiredService<UserManager<ShopUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ShopRole>>();
        var config = services.GetRequiredService<IConfiguration>();

        var roleExist = await roleManager.RoleExistsAsync(AdminRole.RoleName);

        if (!roleExist) {
            var result = await roleManager.CreateAsync(new AdminRole());
        }

        var adminUser = await userManager.FindByEmailAsync(AdminRole.AdminEmail);

        if (adminUser is null) {
            adminUser = new ShopUser {
                UserName = AdminRole.AdminName,
                Email = AdminRole.AdminEmail,
            };
            var adminPassword = config["AdminDefaultPassword"];

            ArgumentException.ThrowIfNullOrWhiteSpace(adminPassword);

            var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminUser.Succeeded) {
                await userManager.AddToRoleAsync(adminUser, AdminRole.RoleName);
                adminUser.EmailConfirmed = true;
                await userManager.UpdateAsync(adminUser);
            }
        }
        return services;
    }
}