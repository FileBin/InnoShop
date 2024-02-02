using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InnoShop.Application.Shared;
using InnoShop.Application.Shared.Auth;
using InnoShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.Infrastructure.UserManagerAPI;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase {
    private readonly IConfiguration Configuration;
    private readonly SignInManager<ShopUser> SignInManager;
    private readonly UserManager<ShopUser> UserManager;

    private readonly SymmetricSecurityKey JwtSecurityKey;

    public AccountsController(IConfiguration configuration, SignInManager<ShopUser> signInManager, UserManager<ShopUser> userManager) {
        UserManager = userManager;
        Configuration = configuration;
        SignInManager = signInManager;
        JwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetOrThrow("JwtSecurityKey")));
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) {
        var username = dto.Login;
        var password = dto.Password;
        var result = await SignInManager.PasswordSignInAsync(username, password, false, false);
        
        if (!result.Succeeded) {
            return BadRequest(new LoginResult { IsSuccessful = false, Message = "Username and password are invalid." });
        }

        var claims = new[] {
            new Claim(ClaimTypes.Name, username)
        };

        var creds = new SigningCredentials(JwtSecurityKey, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(Convert.ToInt32(Configuration.GetOrThrow("JwtExpiryInDays")));

        var token = new JwtSecurityToken(
            Configuration.GetOrThrow("JwtIssuer"),
            Configuration.GetOrThrow("JwtAudience"),
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        return Ok(new LoginResult {
            IsSuccessful = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = username,
        });
    }


    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) {
        var newUser = new ShopUser { UserName = dto.Username, Email = dto.Email };
        var password = dto.Password;

        var result = await UserManager.CreateAsync(newUser, password);

        if (!result.Succeeded) {
            var errors = result.Errors.Select(x => x.Description).Aggregate((x, y) => $"{x}\n{y}");
            return BadRequest(new RegisterResult { IsSuccessful = false, Message = errors });
        }

        return Ok(new RegisterResult { IsSuccessful = true });
    }
}
