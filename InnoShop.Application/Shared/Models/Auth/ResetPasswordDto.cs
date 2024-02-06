using System.Text.Json.Serialization;

namespace InnoShop.Application.Shared.Models.Auth;

public class ResetPasswordDto {
    [JsonConstructor]
    public ResetPasswordDto(string userId, string token, string newPassword) {
        UserId = userId;
        Token = token;
        NewPassword = newPassword;
    }

    public ResetPasswordDto(ResetPasswordDto other) {
        UserId = other.UserId;
        Token = other.Token;
        NewPassword = other.NewPassword;
    }

    public string UserId { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
