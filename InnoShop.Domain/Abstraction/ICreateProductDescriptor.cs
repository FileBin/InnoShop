namespace InnoShop.Domain.Abstraction;

public interface ICreateProductDescriptor : IProductDescriptor {
    public IUserDescriptor UserDesc { get; }
}