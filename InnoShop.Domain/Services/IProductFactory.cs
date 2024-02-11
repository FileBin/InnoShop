namespace InnoShop.Domain.Services;

using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities;

public interface IProductFactory {
    Product Create(ICreateProductDescriptor descriptor);
}