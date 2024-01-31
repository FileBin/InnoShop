using Microsoft.Extensions.Configuration;

namespace InnoShop.Application.Shared;

public static class Misc {
    public static string GetOrThrow(this IConfiguration config, string key) {
        var val = config[key];
        if (val is null) {
            throw new ArgumentException($"Config does not contain {key}");
        }
        return val;
    }
}
