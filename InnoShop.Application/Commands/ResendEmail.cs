using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;

namespace InnoShop.Application.Commands;

public class ResendEmailCommand : IRequest<Result> {
    public required string UserEmail { get; init; }
    public required LinkGenerator ConfirmLinkGenerator { get; init; }
}

public class ResendEmailHandler : IRequestHandler<ResendEmailCommand, Result> {
    private readonly UserManager<ShopUser> userManager;
    private readonly IMediator mediator;

    public ResendEmailHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task<Result> Handle(ResendEmailCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByEmailAsync(request.UserEmail);

        if (user is null) {
            return Result.Fail($"User with email ${request.UserEmail} not found");
        }

        _ = mediator.Send(new SendConfirmationEmailCommand {
            ConfirmLinkGenerator = request.ConfirmLinkGenerator,
            User = user,
        });

        return Result.Ok();
    }
}