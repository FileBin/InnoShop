using InnoShop.Domain.Enums;

namespace InnoShop.Domain.Abstraction;

public interface IProduct : IProductDescriptor {
    Guid Id { get; }
    string UserId { get; }

    AviabilityStatus Aviability { get; }

    public DateTime CreationTimestamp { get; }

    public DateTime LastUpdateTimestamp { get; }
}