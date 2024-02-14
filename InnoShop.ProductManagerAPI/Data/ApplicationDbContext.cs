using InnoShop.Domain.Entities;
using InnoShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Infrastructure.ProductManagerAPI.Data;

public class ApplicationDbContext : DbContext, IProductDbContext {
    public DbSet<Product> Products {
        get {
            return Set<Product>();
        }
    }

    protected Task<int>? SavingTask;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) {
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasPostgresEnum<AvailabilityStatus>();
    }

    public void TriggerSave() {
        SaveChangesAsync().Wait();
    }
}