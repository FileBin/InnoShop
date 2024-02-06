using InnoShop.Application.Commands;
using InnoShop.Application.Shared.Models.Auth;
using InnoShop.Application.Shared.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkGenerator = InnoShop.Application.Shared.Misc.LinkGenerator;

namespace InnoShop.Infrastructure.UserManagerAPI.Controllers;

[ApiController]
[Route("api/accounts", Name = nameof(AccountsController))]
[Authorize]
public class AccountsController : ControllerBase {
    private readonly IMediator mediator;

    public AccountsController(IMediator mediator) {
        this.mediator = mediator;
    }

    [HttpPost]
    [Route("login", Name = nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto,
                                           CancellationToken cancellationToken) {
        var command = new LoginUserCommand() {
            Login = dto.Login,
            Password = dto.Password,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }


    [HttpPost]
    [Route("register", Name = nameof(Register))]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto,
                                              CancellationToken cancellationToken) {
        var command = new RegisterUserCommand() {
            Email = dto.Email,
            Username = dto.Username,
            Password = dto.Password,
            ConfirmLinkGenerator = ConfirmLinkGenerator,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpGet]
    [Route("confirm", Name = nameof(ConfirmEmailAsync))]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId,
                                                       string token,
                                                       CancellationToken cancellationToken) {
        var command = new ConfirmEmailCommand() {
            UserId = userId,
            Token = token,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost]
    [Route("resend", Name = nameof(ResendEmail))]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmail([FromBody] string userEmail,
                                                 CancellationToken cancellationToken) {
        var command = new ResendEmailCommand() {
            UserEmail = userEmail,
            ConfirmLinkGenerator = ConfirmLinkGenerator,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpGet]
    [Route("info", Name = nameof(GetInfo))]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken) {
        var command = new GetUserInfoCommand() {
            User = User,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    private LinkGenerator ConfirmLinkGenerator {
        get => new LinkGenerator {
            ActionName = nameof(ConfirmEmailAsync),
            Url = Url,
        };
    }
}
