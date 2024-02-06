using System.Security.Claims;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;

namespace InnoShop.Application.Commands;

public class GetUserInfoCommand : IRequest<Result<UserInfoDto>> {
    public required ClaimsPrincipal User { get; init; }
}

public class GetUserInfoHandler : IRequestHandler<GetUserInfoCommand, Result<UserInfoDto>> {
    private readonly IMediator mediator;
    private readonly UserManager<ShopUser> userManager;

    public GetUserInfoHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task<Result<UserInfoDto>> Handle(GetUserInfoCommand request,
                                                  CancellationToken cancellationToken) {
        var user = await userManager.GetUserAsync(request.User);

        if (user is null) {
            return Result.Fail("User not found!");
        }

        return Result.Ok(new UserInfoDto {
            Email = user.Email ?? Util.NullMarker,
            Username = user.UserName ?? Util.NullMarker,
        });
    }
}