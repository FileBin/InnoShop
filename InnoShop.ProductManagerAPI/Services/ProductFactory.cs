using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities;
using InnoShop.Domain.Services;

namespace InnoShop.Infrastructure.ProductManagerAPI.Services;

public class ProductFactory : IProductFactory {
    public Product Create(ICreateProductDescriptor descriptor) {
        var Now = DateTime.UtcNow;
        return new Product {
            Id = new Guid(),
            Aviability = AviabilityStatus.Draft,
            CreationTimestamp = Now,
            LastUpdateTimestamp = Now,

            Price = descriptor.Price,
            Title = descriptor.Title,
            Description = descriptor.Description,

            UserId = descriptor.UserDesc.UserId, 
        };
    }
}