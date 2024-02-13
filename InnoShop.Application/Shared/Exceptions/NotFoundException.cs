namespace InnoShop.Application.Shared.Exceptions;

public class NotFoundException : InnoShopException {
    private static readonly string title = "NotFound";

    public static NotFoundException ParameterNotFound(string name) => new NotFoundException($"parameter {name} not found");

    public static NotFoundException NotFoundInDatabase(string name) => new NotFoundException($"{name} not found in database");

    public NotFoundException() : base(title) { }
    public NotFoundException(string? message) : base(title, message) { }
    public NotFoundException(string? message, Exception? innerException) : base(title, message, innerException) { }
}