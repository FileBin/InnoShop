using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.Application.Commands;

public class LoginUserCommand : LoginDto, IRequest<Result<LoginResultDto>> {}

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDto>> {
    private readonly UserManager<ShopUser> userManager;
    private readonly SignInManager<ShopUser> signInManager;
    private readonly IConfiguration config;
    private readonly SymmetricSecurityKey jwtSecurityKey;

    public LoginUserHandler(UserManager<ShopUser> userManager,
                            SignInManager<ShopUser> signInManager,
                            IConfiguration configuration) {
        this.userManager = userManager;
        this.signInManager = signInManager;
        config = configuration;
        jwtSecurityKey = configuration.GetSecurityKey();
    }

    public async Task<Result<LoginResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken) {
        var username = request.Login;
        var password = request.Password;
        var result = await signInManager.PasswordSignInAsync(username, password, false, false);

        if (!result.Succeeded) {
            if (result.IsNotAllowed) {
                return Result.Fail("Username or password are invalid");
            } 
            return Result.Fail("Failed to login");
        }

        var user = await userManager.FindByNameAsync(username);
        ArgumentNullException.ThrowIfNull(user);

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.UserName ?? Util.NullMarker),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var creds = new SigningCredentials(jwtSecurityKey, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(Convert.ToInt32(config.GetOrThrow("JwtExpiryInDays")));

        var token = new JwtSecurityToken(
            config.GetOrThrow("JwtIssuer"),
            config.GetOrThrow("JwtAudience"),
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        return Result.Ok(
            new LoginResultDto() {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = username
            }
        );
    }
}