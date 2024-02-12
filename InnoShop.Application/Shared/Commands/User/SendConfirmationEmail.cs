using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;
using InnoShop.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace InnoShop.Application.Shared.Commands.User;

public class SendConfirmationEmailCommand : ICommand {
    public required ShopUser User { get; init; }
}

public sealed class SendConfirmationEmailValidator : AbstractValidator<SendConfirmationEmailCommand> {
    public SendConfirmationEmailValidator() {
        RuleFor(x => x.User).NotNull();
    }
}

public class SendConfirmationEmailHandler : IUserCommandHandler<SendConfirmationEmailCommand> {
    private readonly UserManager<ShopUser> userManager;
    private readonly IConfirmationMailService mailService;
    private readonly IConfiguration config;

    public SendConfirmationEmailHandler(UserManager<ShopUser> userManager,
                                        IConfirmationMailService mailService,
                                        IConfiguration config) {
        this.userManager = userManager;
        this.mailService = mailService;
        this.config = config;
    }

    public async Task Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken) {
        var user = request.User;

        if (user.EmailConfirmed) {
            throw new BadRequestException("Already confirmed!");
        }

        var url = new RouteBasedLinkGenerator() {
            Route = config.GetOrThrow("ConfirmEmailRoute"),
        };

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = url.GenerateLink(new { userId = user.Id, token });

        ArgumentNullException.ThrowIfNull(confirmationLink);
        ArgumentNullException.ThrowIfNull(user.Email);

        await mailService.SendConfirmationEmailAsync(user.Email, user.Id, confirmationLink);
    }
}