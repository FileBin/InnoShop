namespace InnoShop.Domain.Abstraction;

public interface ITokenPair {
    string AccessToken { get; }
    string RefreshToken { get; }
}