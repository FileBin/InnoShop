using InnoShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Infrastructure.UserManagerAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ShopUser, IdentityRole, string> {
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
}