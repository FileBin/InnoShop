using InnoShop.Domain.Abstraction;

namespace InnoShop.Application.Shared.Models.Auth;

public class LoginResultDto : ITokenPair {
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
