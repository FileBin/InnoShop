using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Validation;
using InnoShop.Domain;

namespace InnoShop.Application.Shared.Commands;

public class RegisterUserCommand : RegisterDto, ICommand {
    public RegisterUserCommand(RegisterDto other) : base(other) {}

    public required LinkGenerator ConfirmLinkGenerator { get; init; }
}

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand> {
    public RegisterUserValidator() {
        RuleFor(x => x.ConfirmLinkGenerator).NotNull();
        RuleFor(x => x.Email).EmailValidation();
        RuleFor(x => x.Username).UsernameValidation();
        RuleFor(x => x.Password).PasswordValidation();
    }
}

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand> {
    private readonly IMediator mediator;
    private readonly UserManager<ShopUser> userManager;

    public RegisterUserHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken) {
        var newUser = new ShopUser { UserName = request.Username, Email = request.Email };

        var result = await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) {
            var errors = result.Errors.Select(x => x.Description).Aggregate((x, y) => $"{x}\n{y}");
            throw new BadRequestException(errors);
        }

        newUser = await userManager.FindByNameAsync(newUser.UserName);
        ArgumentNullException.ThrowIfNull(newUser);

        _ = mediator.Send(new SendConfirmationEmailCommand{
            ConfirmLinkGenerator = request.ConfirmLinkGenerator,
            User = newUser,
        });
    }
}