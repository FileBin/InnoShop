using InnoShop.Domain.Entities;
using InnoShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Infrastructure.ProductManagerAPI.Data;

public class ApplicationDbContext : DbContext, IProductDbContext {
    public DbSet<Product> Products {
        get {
            SavingTask?.Wait(30_000);
            return Set<Product>();
        }
    }

    protected Task<int>? SavingTask;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasPostgresEnum<AvailabilityStatus>();
    }

    public void TriggerSave() {
        if (SavingTask?.IsCompleted ?? true) {
            SavingTask = SaveChangesAsync();
        }
    }
}