using Microsoft.AspNetCore.Identity;

namespace InnoShop.Domain.Entities.Roles;

public class ShopRole : IdentityRole {
    public ShopRole() { }

    public ShopRole(string roleName) : base(roleName) { }
}