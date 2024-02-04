namespace InnoShop.Application.Shared.Auth;

public class BaseResult {
    public required bool IsSuccessful { get; set; }
    public string? Message { get; set; }
}
