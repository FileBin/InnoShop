using InnoShop.Application.Shared.Exceptions;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Enums;

namespace InnoShop.Application.Shared.Misc;

public static class ProductAccessExtensions {
    public static bool IsOwner(this IUserDescriptor user, IProduct product) => user.UserId == product.UserId;

    public static IQueryable<TProduct> FilterSearch<TProduct>(this IQueryable<TProduct> productsQuery, IUserDescriptor user)
    where TProduct : IProduct {
        if(user.IsAdmin) {
            return productsQuery;
        }
        return productsQuery.Where(product => product.Aviability == AviabilityStatus.Published);
    }

    public static bool IsVisibleToUser(this IProduct product, IUserDescriptor user) {
        if (user.IsAdmin) return true;

        switch (product.Aviability) {
            case AviabilityStatus.Draft:
                return user.IsOwner(product);
            case AviabilityStatus.Published:
            case AviabilityStatus.Sold:
                return true;
        }

        return false;
    }

    public static bool IsEditableByUser(this IProduct product, IUserDescriptor user) {
        if (user.IsAdmin) return true;

        return user.IsOwner(product);
    }

    /// <summary>
    /// That function validates if user can see product otherwise throws exception
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="user">User that tries to see product</param>
    /// <exception cref="NotFoundException">Product is not visible to user</exception>
    public static void ValidateVisibility(this IProduct product, IUserDescriptor user) {
        if (product.IsVisibleToUser(user)) {
            return;
        }
        throw NotFoundException.NotFoundInDatabase("Product");
    }

    /// <summary>
    /// That function validates if user can edit product otherwise throws exception
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="user">User that tries to edit product</param>
    /// <exception cref="NotFoundException">Product is not editable by user</exception>
    public static void ValidateEdit(this IProduct product, IUserDescriptor user) {
        if (product.IsEditableByUser(user)) {
            return;
        }
        throw NotFoundException.NotFoundInDatabase("Product");
    }
}