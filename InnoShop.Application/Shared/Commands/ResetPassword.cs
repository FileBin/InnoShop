using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Domain;

namespace InnoShop.Application.Shared.Commands;

public class ResetPasswordCommand : ResetPasswordDto, ICommand {
    public ResetPasswordCommand(ResetPasswordDto other) : base(other) { }
    public ResetPasswordCommand(string userId, string token, string newPassword) : base(userId, token, newPassword) { }
}

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand> {
    public ResetPasswordValidator() {
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Token).NotEmpty().MaximumLength(1024);
    }
}

public class ResetPasswordHandler : IUserCommandHandler<ResetPasswordCommand> {
    private readonly UserManager<ShopUser> userManager;

    public ResetPasswordHandler(UserManager<ShopUser> userManager) {
        this.userManager = userManager;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null) {
            throw new NotFoundException("User not found");
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded) {
            throw new BadRequestException(result.Errors.Select(x => x.ToString()).Aggregate((x, y) => $"{x}\n{y}"));
        }
    }
}