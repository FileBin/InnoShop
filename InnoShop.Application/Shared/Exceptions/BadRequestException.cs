namespace InnoShop.Application.Shared.Exceptions;

public class BadRequestException : InnoShopException {
    private static readonly string title = "BadRequest";
    public BadRequestException(): base(title) { }
    public BadRequestException(string? message) : base(title, message) { }
    public BadRequestException(string? message, Exception? innerException) : base(title, message, innerException) { }
}