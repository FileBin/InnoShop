using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Validation;
using InnoShop.Domain;
using InnoShop.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace InnoShop.Application.Shared.Commands.User;

public class ForgotPasswordCommand : ICommand {
    public required string UserEmail { get; init; }
}

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand> {
    public ForgotPasswordValidator() {
        RuleFor(x => x.UserEmail).EmailValidation();
    }
}

public class ForgotPasswordHandler : IUserCommandHandler<ForgotPasswordCommand> {
    private readonly UserManager<ShopUser> userManager;

    private readonly IPasswordResetMailService mailService;

    private readonly IConfiguration config;



    public ForgotPasswordHandler(UserManager<ShopUser> userManager,
                                 IPasswordResetMailService mailService,
                                 IConfiguration config) {
        this.userManager = userManager;
        this.mailService = mailService;
        this.config = config;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByEmailAsync(request.UserEmail);

        if (user is null) {
            throw new NotFoundException($"User with email ${request.UserEmail} not found");
        }

        if (!user.EmailConfirmed) {
            throw new BadRequestException("User email is not confirmed!");
        }

        var url = new RouteBasedLinkGenerator() {
            Route = config.GetOrThrow("PasswordResetRoute"),
        };

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var PasswordResetLink = url.GenetareLink(new { userId = user.Id, token });

        ArgumentNullException.ThrowIfNull(PasswordResetLink);
        ArgumentNullException.ThrowIfNull(user.Email);

        await mailService.SendPasswordResetEmailAsync(user.Email, user.Id, PasswordResetLink);
    }
}