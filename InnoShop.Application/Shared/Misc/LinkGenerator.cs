namespace InnoShop.Application.Shared.Misc;

using InnoShop.Application.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LinkGenerator : ILinkGenerator {
    public required string ActionName { get; init; }
    public required IUrlHelper Url { get; init; }

    public string GenerateLink(object? values = null) {
        var link = Url.Link(ActionName, values);
        ArgumentNullException.ThrowIfNull(link);
        return link;
    }
}