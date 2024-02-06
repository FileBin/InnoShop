using InnoShop.Application.Shared.Misc;
using InnoShop.Domain;
using InnoShop.Domain.Services;

namespace InnoShop.Application.Commands;

public class SendConfirmationEmailCommand : IRequest<Result> {
    public required ShopUser User { get; init; }
    public required LinkGenerator ConfirmLinkGenerator { get; init; }
}

public class SendConfirmationEmailHandler : IRequestHandler<SendConfirmationEmailCommand, Result> {
    private readonly UserManager<ShopUser> userManager;
    private readonly IConfirmationMailService mailService;

    public SendConfirmationEmailHandler(UserManager<ShopUser> userManager,
                                        IConfirmationMailService mailService) {
        this.userManager = userManager;
        this.mailService = mailService;
    }

    public async Task<Result> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken) {
        var user = request.User;
        var url = request.ConfirmLinkGenerator;

        if (user.EmailConfirmed) return Result.Fail("Already confirmed!");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = url.GenetareLink(new { userId = user.Id, token });

        ArgumentNullException.ThrowIfNull(confirmationLink);
        ArgumentNullException.ThrowIfNull(user.Email);

        await mailService.SendConfirmationEmailAsync(user.Email, user.Id, confirmationLink);
        
        return Result.Ok();
    }
}