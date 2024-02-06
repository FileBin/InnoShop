namespace InnoShop.Application.Shared.Misc;

using Microsoft.AspNetCore.Mvc;

public class LinkGenerator {
    public required string ActionName { get; init; }
    public required IUrlHelper Url { get; init; }

    public string GenetareLink(object? values = null) {
        var link = Url.Link(ActionName, values);
        ArgumentNullException.ThrowIfNull(link);
        return link;
    }
}