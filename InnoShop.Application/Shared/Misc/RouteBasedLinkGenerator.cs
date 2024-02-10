namespace InnoShop.Application.Shared.Misc;

using InnoShop.Application.Shared.Interfaces;

public class RouteBasedLinkGenerator : ILinkGenerator {
    public required string Route { get; init; }

    public string GenerateLink(object? values = null) {
        var link = Route.Trim();
        if (link.EndsWith('/')) {
            link = link[..^1];
        }
        if (values is not null) {
            link = $"{link}?{Util.AnyToUrlQuery(values)}";
        }
        return link;
    }
}
