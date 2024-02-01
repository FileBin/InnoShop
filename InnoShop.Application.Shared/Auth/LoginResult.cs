namespace InnoShop.Application.Shared.Auth;

public class LoginResult {
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }

    public string? Username { get; set; }
}
