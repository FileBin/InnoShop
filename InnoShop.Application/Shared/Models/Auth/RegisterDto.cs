using System.Text.Json.Serialization;

namespace InnoShop.Application.Shared.Models.Auth;

public class RegisterDto {
    [JsonConstructor]
    public RegisterDto(string email, string username, string password) {
        Email = email;
        Username = username;
        Password = password;
    }

    public RegisterDto(RegisterDto other) {
        Email = other.Email;
        Username = other.Username;
        Password = other.Password;
    }

    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
