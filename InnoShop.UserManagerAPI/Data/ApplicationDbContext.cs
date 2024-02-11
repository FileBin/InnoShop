using InnoShop.Domain;
using InnoShop.Domain.Entities.Roles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Infrastructure.UserManagerAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ShopUser, ShopRole, string> {
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
}