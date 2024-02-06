using System.Collections.Concurrent;
using System.Net.Mail;
using InnoShop.Domain.Services;

namespace InnoShop.Tests.TestUserAPI;

class TestMailService : IConfirmationMailService {
    static ConcurrentBag<MailMessage> testEmailStorage = new ConcurrentBag<MailMessage>();

    public static IEnumerable<MailMessage> EmailStorage { get => testEmailStorage; }

    public static bool PeekMailFromHeap(out MailMessage? message) => testEmailStorage.TryPeek(out message);
    public static void ClearStorage() => testEmailStorage.Clear();

    public Task SendEmailAsync(string destination, string subject, string message) {
        testEmailStorage.Add(
            new MailMessage(
                from: "test@example.com",
                to: destination,
                subject,
                message));

        return Task.CompletedTask;
    }

    public async Task SendConfirmationEmailAsync(string userEmail, string userId, string confirmationLink) {
        await SendEmailAsync(
           destination: userEmail,
           subject: "InnoShop email confirmation",
           message: confirmationLink);
    }
}