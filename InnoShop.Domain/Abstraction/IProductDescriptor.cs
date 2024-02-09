using InnoShop.Domain.Enums;

namespace InnoShop.Domain.Abstraction;

public interface IProductDescriptor {
    public string Title { get; }
    public string Description { get; }
    public decimal Price { get; }
}

public interface IProductDescriptorNullable {
    public string? Title { get; }
    public string? Description { get; }
    public decimal? Price { get; }

    public AviabilityStatus? Status { get; }
}