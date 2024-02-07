using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Domain;
using InnoShop.Domain.Services;

namespace InnoShop.Application.Shared.Commands.User;

public class SendConfirmationEmailCommand : ICommand {
    public required ShopUser User { get; init; }
    public required ILinkGenerator ConfirmLinkGenerator { get; init; }
}

public sealed class SendConfirmationEmailValidator : AbstractValidator<SendConfirmationEmailCommand> {
    public SendConfirmationEmailValidator() {
        RuleFor(x => x.ConfirmLinkGenerator).NotNull();
        RuleFor(x => x.User).NotNull();
    }
}

public class SendConfirmationEmailHandler : IUserCommandHandler<SendConfirmationEmailCommand> {
    private readonly UserManager<ShopUser> userManager;
    private readonly IConfirmationMailService mailService;

    public SendConfirmationEmailHandler(UserManager<ShopUser> userManager,
                                        IConfirmationMailService mailService) {
        this.userManager = userManager;
        this.mailService = mailService;
    }

    public async Task Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken) {
        var user = request.User;
        var url = request.ConfirmLinkGenerator;

        if (user.EmailConfirmed) {
            throw new BadRequestException("Already confirmed!");
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = url.GenetareLink(new { userId = user.Id, token });

        ArgumentNullException.ThrowIfNull(confirmationLink);
        ArgumentNullException.ThrowIfNull(user.Email);

        await mailService.SendConfirmationEmailAsync(user.Email, user.Id, confirmationLink);
    }
}