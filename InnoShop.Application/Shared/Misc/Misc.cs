using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.Application.Shared.Misc;

public static class Util {

    public static readonly string NullMarker = "(null)";
    public static string GetOrThrow(this IConfiguration config, string key) {
        var val = config[key];
        if (val is null) {
            throw new ArgumentException($"Config does not contain {key}");
        }
        return val;
    }

    public static SymmetricSecurityKey GetSecurityKey(this IConfiguration config) {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetOrThrow("JwtSecurityKey")));
    }
}
