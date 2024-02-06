using InnoShop.Domain;

namespace InnoShop.Application.Commands;

public class ConfirmEmailCommand : IRequest<Result> {
    public required string UserId { get; init; }
    public required string Token { get; init; }
}

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result> {
    private readonly UserManager<ShopUser> userManager;

    public ConfirmEmailHandler(UserManager<ShopUser> userManager) {
        this.userManager = userManager;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null) {
            return Result.Fail("User not found");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded) {
            return Result.Fail("Bad token");
        }
        
        return Result.Ok();
    }
}