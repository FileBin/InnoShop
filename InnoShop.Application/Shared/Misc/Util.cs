using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

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

    public static string DtoToUrlQuery(object o) {
        var pairs = o.GetType().GetProperties()
            .Select(p => {
                var altName = p.GetCustomAttribute<BindPropertyAttribute>()?.Name;
                var Name = altName ?? p.Name;
                return new { Name, Value = p.GetValue(o, null) };
            })
            .Where(p => p.Value != null)
            .Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.Value!.ToString()))
            .ToArray();

        return string.Join("&", pairs);
    }

    public static string AnyToUrlQuery(object o) {
        if (o.GetType().GetProperties().Length > 0)
            return DtoToUrlQuery(o);
            
        return AnonymousToUrlQuery(o);

    }

    public static string AnonymousToUrlQuery(object o) {
        var json = JsonConvert.SerializeObject(o);
        var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(o);
        var pairs = dict.Select(x => {
            var str = x.Value?.ToString();
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
