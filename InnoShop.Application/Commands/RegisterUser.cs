using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Domain;

namespace InnoShop.Application.Commands;

public class RegisterUserCommand : RegisterDto, IRequest<Result> {
    public required LinkGenerator ConfirmLinkGenerator { get; init; }
}

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result> {
    private readonly IMediator mediator;
    private readonly UserManager<ShopUser> userManager;

    public RegisterUserHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken) {
        var newUser = new ShopUser { UserName = request.Username, Email = request.Email };

        var result = await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) {
            var errors = result.Errors.Select(x => x.Description).Aggregate((x, y) => $"{x}\n{y}");
            return Result.Fail(errors);
        }

        newUser = await userManager.FindByNameAsync(newUser.UserName);
        ArgumentNullException.ThrowIfNull(newUser);

        _ = mediator.Send(new SendConfirmationEmailCommand{
            ConfirmLinkGenerator = request.ConfirmLinkGenerator,
            User = newUser,
        });

        return Result.Ok();
    }
}