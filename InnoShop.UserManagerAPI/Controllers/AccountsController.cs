using InnoShop.Application.Shared.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkGenerator = InnoShop.Application.Shared.Misc.LinkGenerator;
using InnoShop.Application.Shared.Commands.User;

namespace InnoShop.Infrastructure.UserManagerAPI.Controllers;

[ApiController]
[Route("api/accounts", Name = nameof(AccountsController))]
[Authorize]
public class AccountsController : ControllerBase {
    private readonly IMediator mediator;

    private LinkGenerator ConfirmLinkGenerator {
        get => new LinkGenerator {
            ActionName = nameof(ConfirmEmailAsync),
            Url = Url,
        };
    }

    public AccountsController(IMediator mediator) {
        this.mediator = mediator;
    }

    [HttpPost]
    [Route("login", Name = nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto,
                                           CancellationToken cancellationToken) {
        var command = new LoginUserCommand(dto);

        var response = await mediator.Send(command, cancellationToken);

        return Ok(response);
    }


    [HttpPost]
    [Route("register", Name = nameof(Register))]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto,
                                              CancellationToken cancellationToken) {
        var command = new RegisterUserCommand(dto) {
            ConfirmLinkGenerator = ConfirmLinkGenerator,
        };

        await mediator.Send(command, cancellationToken);

        return Ok();
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

        await mediator.Send(command, cancellationToken);

        return Ok();
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

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("forgot_password", Name = nameof(ForgotPassword))]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] string userEmail,
                                         CancellationToken cancellationToken) {
        var command = new ForgotPasswordCommand() {
            UserEmail = userEmail,
        };

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("reset_password", Name = nameof(ResetPassword))]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto,
                                                   CancellationToken cancellationToken) {
        var command = new ResetPasswordCommand(dto);

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    [Route("info", Name = nameof(GetInfo))]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken) {
        var command = new GetUserInfoCommand() {
            User = User,
        };

        var response = await mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}
