using InnoShop.Domain.Enums;

namespace InnoShop.Domain.Abstraction;

public interface IProduct : IProductDescriptor {
    Guid Id { get; }
    string UserId { get; }

    AvailabilityStatus Availability { get; }

    public DateTime CreationTimestamp { get; }

    public DateTime LastUpdateTimestamp { get; }
}