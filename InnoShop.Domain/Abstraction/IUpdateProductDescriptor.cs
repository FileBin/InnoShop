namespace InnoShop.Domain.Abstraction;

public interface IUpdateProductDescriptor : IProductDescriptorNullable {
    string ProductId { get; }
    IUserDescriptor UserDesc { get; }
}