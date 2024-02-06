using System.Net;
using System.Net.Mail;
using InnoShop.Domain.Services;
using InnoShop.Application.Shared.Misc;

namespace InnoShop.Infrastructure.UserManagerAPI.Services;

public class MailService : IConfirmationMailService {
    IConfiguration Configuration;
    public MailService(IConfiguration configuration) {
        Configuration = configuration;
    }

    public async Task SendConfirmationEmailAsync(string userEmail, string userId, string confirmationLink) {
        var mailText = $"Please confirm your E-Mail\n"
                     + $"{confirmationLink}";

        await SendEmailAsync(
           destination: userEmail,
           subject: "InnoShop email confirmation",
           message: mailText);
    }

    public async Task SendEmailAsync(string destination, string subject, string message) {
        var conf = new {
            host = Configuration.GetOrThrow("SMTP:Host"),
            port = Convert.ToInt32(Configuration.GetOrThrow("SMTP:Port")),
            user = Configuration.GetOrThrow("SMTP:User"),
            password = Configuration.GetOrThrow("SMTP:Password"),
        };

        var client = new SmtpClient(conf.host, conf.port) {
            EnableSsl = true,
            Credentials = new NetworkCredential(conf.user, conf.password),
        };

        await client.SendMailAsync(new MailMessage(from: conf.user, to: destination, subject, message));
    }
}
