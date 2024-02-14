using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using InnoShop.Application.Validation;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Exceptions;
using InnoShop.Domain.Entities.Roles;
using InnoShop.Domain.Services;

namespace InnoShop.Application.Shared.Commands.User;

public class LoginUserCommand : LoginDto, ICommand<LoginResultDto> {
    public LoginUserCommand(LoginDto other) : base(other) { }
}

public sealed class LoginUserValidator : AbstractValidator<LoginUserCommand> {
    public LoginUserValidator() {
        RuleFor(x => x.Login).LoginValidation();
        RuleFor(x => x.Password).PasswordValidation();
    }
}

public class LoginUserHandler(UserManager<ShopUser> userManager,
                        SignInManager<ShopUser> signInManager,
                        ITokenService tokenService) 
                        
                        : IUserCommandHandler<LoginUserCommand, LoginResultDto> {


    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken) {
        var user = await userManager.FindByNameAsync(request.Login);
        user = user ?? await userManager.FindByEmailAsync(request.Login);

        if (user is null) {
            throw new BadRequestException("Username or password are invalid");
        }

        var password = request.Password;
        var result = await signInManager.PasswordSignInAsync(user, password, false, false);

        if (!result.Succeeded) {
            throw new BadRequestException("Username or password are invalid");
        }

        

        var pair = await tokenService.GenerateTokenAsync(user);

        return new LoginResultDto {
            AccessToken = pair.AccessToken,
            RefreshToken = pair.RefreshToken,
        };
    }
}