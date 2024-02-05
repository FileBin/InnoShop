namespace InnoShop.Domain.Services;

public interface IConfirmationMailService : IMailService {
    public Task SendConfirmationEmailAsync(string userEmail, string userId, string confirmationLink);
}
