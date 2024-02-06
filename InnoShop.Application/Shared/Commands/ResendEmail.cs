using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Validation;
using InnoShop.Domain;

namespace InnoShop.Application.Shared.Commands;

public class ResendEmailCommand : ICommand {
    public required string UserEmail { get; init; }
    public required LinkGenerator ConfirmLinkGenerator { get; init; }
}

public sealed class ResendEmailValidator : AbstractValidator<ResendEmailCommand> {
    public ResendEmailValidator() {
        RuleFor(x => x.ConfirmLinkGenerator).NotNull();
        RuleFor(x => x.UserEmail).EmailValidation();
    }
}

public class ResendEmailHandler : ICommandHandler<ResendEmailCommand> {
    private readonly UserManager<ShopUser> userManager;
    private readonly IMediator mediator;

    public ResendEmailHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task Handle(ResendEmailCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByEmailAsync(request.UserEmail);

        if (user is null) {
            throw new NotFoundException($"User with email ${request.UserEmail} not found");
        }

        _ = mediator.Send(new SendConfirmationEmailCommand {
            ConfirmLinkGenerator = request.ConfirmLinkGenerator,
            User = user,
        });
    }
}