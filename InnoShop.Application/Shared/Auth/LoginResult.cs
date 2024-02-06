namespace InnoShop.Application.Shared.Auth;

public class LoginResult : BaseResult {

    public string? Token { get; set; }

    public string? Username { get; set; }
}
