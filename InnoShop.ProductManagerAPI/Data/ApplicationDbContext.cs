using InnoShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Infrastructure.ProductManagerAPI.Data;

public class ApplicationDbContext : DbContext, IProductDbContext {
    public DbSet<Product> Products => Set<Product>();

    Task<int>? SavingTask;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) {
        Database.EnsureCreated();
    }

    public void TriggerSave() {
        if (SavingTask is null || SavingTask.IsCompleted) {
            SavingTask = SaveChangesAsync();
        }
    }
}