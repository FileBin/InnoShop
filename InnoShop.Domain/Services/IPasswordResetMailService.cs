namespace InnoShop.Domain.Services;

public interface IPasswordResetMailService : IMailService {
    public Task SendPasswordResetEmailAsync(string userEmail, string userId, string passwordResetLink);
}
