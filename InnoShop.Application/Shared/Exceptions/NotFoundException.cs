namespace InnoShop.Application.Shared.Exceptions;

public class NotFoundException : InnoShopException {
    private static readonly string title = "NotFound";
    public NotFoundException(): base(title) { }
    public NotFoundException(string? message) : base(title, message) { }
    public NotFoundException(string? message, Exception? innerException) : base(title, message, innerException) { }
}