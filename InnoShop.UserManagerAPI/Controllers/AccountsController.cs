using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InnoShop.Application.Shared;
using InnoShop.Application.Shared.Auth;
using InnoShop.Domain;
using InnoShop.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.Infrastructure.UserManagerAPI.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountsController : ControllerBase {
    private readonly IConfiguration Configuration;
    private readonly IWebHostEnvironment Environment;
    private readonly SignInManager<ShopUser> SignInManager;
    private readonly UserManager<ShopUser> UserManager;
    private readonly IConfirmationMailService MailService;

    private readonly SymmetricSecurityKey JwtSecurityKey;

    public AccountsController(IConfiguration configuration,
                              IWebHostEnvironment environment,
                              SignInManager<ShopUser> signInManager,
                              UserManager<ShopUser> userManager,
                              IConfirmationMailService mailService) {
        Configuration = configuration;
        Environment = environment;
        SignInManager = signInManager;
        UserManager = userManager;
        MailService = mailService;
        JwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetOrThrow("JwtSecurityKey")));
    }

    [HttpPost]
    [Route("login", Name = nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) {
        var username = dto.Login;
        var password = dto.Password;
        var result = await SignInManager.PasswordSignInAsync(username, password, false, false);

        if (!result.Succeeded) {
            return BadRequest(new LoginResult {
                IsSuccessful = false,
                Message = "Username and password are invalid."
            });
        }

        var user = (await UserManager.FindByNameAsync(username))!;

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.UserName ?? Misc.NullMarker),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
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
    [Route("register", Name = nameof(Register))]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) {
        var newUser = new ShopUser { UserName = dto.Username, Email = dto.Email };
        var password = dto.Password;

        var result = await UserManager.CreateAsync(newUser, password);

        if (!result.Succeeded) {
            var errors = result.Errors.Select(x => x.Description).Aggregate((x, y) => $"{x}\n{y}");
            return BadRequest(new BaseResult { IsSuccessful = false, Message = errors });
        }

        newUser = await UserManager.FindByNameAsync(newUser.UserName);
        ArgumentNullException.ThrowIfNull(newUser);

        _ = SendConfirmationEmailToUserAsync(newUser);
        return Ok(BaseResult.Success("You registered succesfully. But before login you need to verify your e-mail"));
    }

    [HttpGet]
    [Route("confirm", Name = nameof(ConfirmEmailAsync))]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token) {
        var user = await UserManager.FindByIdAsync(userId);

        if (user is null) {
            return NotFound(BaseResult.Error("User not found"));
        }

        var result = await UserManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded) {
            return BadRequest(BaseResult.Error("Bad token"));
        }

        return Ok(BaseResult.Success("You registered succesfully"));
    }

    [HttpPost]
    [Route("resend", Name = nameof(ResendEmail))]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmail([FromBody] string userEmail) {
        var user = await UserManager.FindByEmailAsync(userEmail);

        if (user is null) {
            return NotFound(BaseResult.Error($"User with email ${userEmail} not found"));
        }

        await SendConfirmationEmailToUserAsync(user);

        return Ok(BaseResult.Success());
    }

    [HttpGet]
    [Route("info", Name = nameof(GetInfo))]
    public async Task<IActionResult> GetInfo() {
        var user = await UserManager.GetUserAsync(User);

        if (user is null) {
            return BadRequest(new BaseResult {
                IsSuccessful = false,
                Message = "User not found!",
            });
        }

        return Ok(new UserInfoDto {
            Email = user.Email ?? Misc.NullMarker,
            Username = user.UserName ?? Misc.NullMarker,
        });
    }

    private async Task SendConfirmationEmailToUserAsync(ShopUser user) {
        if (user.EmailConfirmed) return;

        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        
        var confirmationLink = Url.Link(nameof(ConfirmEmailAsync), new { userId = user.Id, token });
        
        ArgumentNullException.ThrowIfNull(confirmationLink);
        ArgumentNullException.ThrowIfNull(user.Email);

        await MailService.SendConfirmationEmailAsync(user.Email, user.Id, confirmationLink);
    }
}
