using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Domain;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Services;

namespace InnoShop.Application.Shared.Commands.User;

public class RefreshTokenCommand : ICommand<LoginResultDto> {
    public required IUserDescriptor UserDesc { get; set; }
}

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand> {
    public RefreshTokenValidator() {
        RuleFor(x => x.UserDesc.UserId).NotEmpty();
    }
}

public class RefreshTokenCommandHandler(UserManager<ShopUser> userManager,
                                        ITokenService tokenService)
                                        : IUserCommandHandler<RefreshTokenCommand, LoginResultDto> {

    public async Task<LoginResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByIdAsync(request.UserDesc.UserId);

        if (user is null) {
            throw new NotFoundException("Username not found");
        }

        var pair = await tokenService.GenerateTokenAsync(user);

        return new LoginResultDto {
            AccessToken = pair.AccessToken,
            RefreshToken = pair.RefreshToken,
        };
    }
}
