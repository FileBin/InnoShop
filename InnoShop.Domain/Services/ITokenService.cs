using InnoShop.Domain.Abstraction;

namespace InnoShop.Domain.Services;

public interface ITokenService {
    Task<ITokenPair> GenerateTokenAsync(ShopUser user);
}