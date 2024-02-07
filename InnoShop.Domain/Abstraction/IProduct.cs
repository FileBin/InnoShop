using InnoShop.Domain.Entities;

namespace InnoShop.Domain.Abstraction;

public interface IProduct : IProductDescriptor {
    Guid Id { get; }
    string UserId { get; }

    AviabilityStatus Aviability { get; }
}