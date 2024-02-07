using InnoShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IProductDbContext
{
    public DbSet<Product> Products { get; }

    void TriggerSave();

}