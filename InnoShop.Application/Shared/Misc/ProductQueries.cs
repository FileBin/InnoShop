using InnoShop.Application.Shared.Exceptions;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities;
using InnoShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Application.Shared.Misc;

public static class ProductQueries {

    public static async Task<Product> GetProductById(this DbSet<Product> products,
                                             string productId,
                                             CancellationToken cancellationToken = default) {
        return await products.GetProductById(Guid.Parse(productId), cancellationToken);
    }

    public static async Task<Product> GetProductById(this DbSet<Product> products,
                                                     Guid productId,
                                                     CancellationToken cancellationToken = default) {
        var product = await products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);

        if (product is null)
            throw NotFoundException.NotFoundInDatabase("Product");

        return product;
    }

    public static async Task<IEnumerable<Guid>> SearchProductsIds(this DbSet<Product> db,
                                                 ISearchQuery searchQuery,
                                                 IUserDescriptor user,
                                                 CancellationToken cancellationToken = default) {

        var contains = searchQuery.Contains?.Trim().ToLower();
        var priceFrom = searchQuery.PriceFrom;
        var priceUpTo = searchQuery.PriceUpTo;

        var products = db
            .AsNoTracking();

        products.FilterSearch(user);

        if (priceFrom is not null) {
            products = products.Where(product
                => product.Price >= priceFrom.Value);
        }

        if (priceUpTo is not null) {
            products = products.Where(product
                => product.Price <= priceUpTo.Value);
        }

        if (contains is not null) {
            products = products.Where(product
                => product.Title.ToLower().Contains(contains));
        }

        switch (searchQuery.SortingOrder) {
            case SortingOrder.AtoZ:
                products = products.OrderBy(product => product.Title.ToLower());
                break;
            case SortingOrder.ByDate:
                products = products.OrderBy(product => product.CreationTimestamp);
                break;
            case SortingOrder.ByPrice:
                products = products.OrderBy(product => product.Price);
                break;
        }

        if (searchQuery.SortingType == SortingType.Descending) {
            products = products.Reverse();
        }

        var skip = searchQuery.From;
        var take = searchQuery.To - searchQuery.From + 1;

        products = products.Skip(skip).Take(take);

        return await products
            .Select(product => product.Id)
            .ToListAsync(cancellationToken);
    }
}