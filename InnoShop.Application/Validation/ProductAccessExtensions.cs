using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities;

namespace InnoShop.Application.Shared.Misc;

public static class ProductAccessExtensions {
    public static bool IsOwner(this IUserDescriptor user, IProduct product) => user.UserId == product.UserId;

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
}