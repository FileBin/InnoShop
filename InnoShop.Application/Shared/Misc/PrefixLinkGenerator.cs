namespace InnoShop.Application.Shared.Misc;

using InnoShop.Application.Shared.Interfaces;

public class RouteBasedLinkGenerator : ILinkGenerator {
    public required string Route { get; init; }

    public string GenetareLink(object? values = null) {
        var link = Route.Trim();
        if (link.EndsWith("/")) {
            link = link.Substring(0, link.Length - 1);
        }
        if (values is not null) {
            link = $"{link}?{Util.AnnonymousToUrlQuery(values)}";
        }
        return link;
    }
}