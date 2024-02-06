using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Domain;

namespace InnoShop.Application.Shared.Commands;

public class ConfirmEmailCommand : ICommand {
    public required string UserId { get; init; }
    public required string Token { get; init; }
}

public sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand> {
    public ConfirmEmailValidator() {
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Token).NotEmpty().MaximumLength(1024);
    }
}

public class ConfirmEmailHandler : IUserCommandHandler<ConfirmEmailCommand> {
    private readonly UserManager<ShopUser> userManager;

    public ConfirmEmailHandler(UserManager<ShopUser> userManager) {
        this.userManager = userManager;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null) {
            throw new NotFoundException("User not found");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded) {
            throw new BadRequestException("Bad token");
        }
    }
}