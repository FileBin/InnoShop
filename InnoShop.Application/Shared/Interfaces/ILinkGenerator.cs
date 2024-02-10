namespace InnoShop.Application.Shared.Interfaces;

public interface ILinkGenerator {
    public string GenerateLink(object? values = null);
}