using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.Application.Shared.Misc;

public static class Util {
    public static readonly string NullMarker = "(null)";

    public static readonly int MaxQuery = 1024;


    public static string GetOrThrow(this IConfiguration config, string key) {
        var val = config[key];
        if (val is null) {
            throw new ArgumentException($"Config does not contain {key}");
        }
        return val;
    }

    public static SymmetricSecurityKey GetSecurityKey(this IConfiguration config) {
        return new SymmetricSecurityKey(Convert.FromBase64String(config.GetOrThrow("JwtSecurityKey")));
    }

    public static string AnonymousToUrlQuery(object o) {
        var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(o);
        var pairs = dict.Select(x => {
            var str = x.Value.ToString();
            if (str is not null) {
                return $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(str)}";
            }
            return null;
        })
        .Where(x => x is not null)
        .ToArray();
        return string.Join("&", pairs);
    }
}
