using System.Security.Claims;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities.Roles;

namespace InnoShop.Application.Shared.Misc;

public class ClaimUserDescriptor : IUserDescriptor {
    public required ClaimsPrincipal User { get; init; }

    public static ClaimUserDescriptor From(ClaimsPrincipal user) {
        return new ClaimUserDescriptor() { User = user };
    }

    public string UserId
        => User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value!;

    public bool IsAdmin => User.IsInRole(AdminRole.RoleName);
}