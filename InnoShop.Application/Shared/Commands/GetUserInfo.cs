using System.Security.Claims;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;
using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;

namespace InnoShop.Application.Shared.Commands;

public class GetUserInfoCommand : ICommand<UserInfoDto> {
    public required ClaimsPrincipal User { get; init; }
}

public sealed class GetUserInfoValidator : AbstractValidator<GetUserInfoCommand> {
    public GetUserInfoValidator() {
        RuleFor(x => x.User).NotNull();
    }
}

public class GetUserInfoHandler : ICommandHandler<GetUserInfoCommand, UserInfoDto> {
    private readonly IMediator mediator;
    private readonly UserManager<ShopUser> userManager;

    public GetUserInfoHandler(UserManager<ShopUser> userManager, IMediator mediator) {
        this.userManager = userManager;
        this.mediator = mediator;
    }

    public async Task<UserInfoDto> Handle(GetUserInfoCommand request,
                                                  CancellationToken cancellationToken) {
        var user = await userManager.GetUserAsync(request.User);

        if (user is null) {
            throw new NotFoundException("User not found!");
        }

        return new UserInfoDto {
            Email = user.Email ?? Util.NullMarker,
            Username = user.UserName ?? Util.NullMarker,
        };
    }
}