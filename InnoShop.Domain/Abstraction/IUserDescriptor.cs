namespace InnoShop.Domain.Abstraction;

public interface IUserDescriptor {
    string UserId { get; }
    bool IsAdmin { get; }
}