namespace InnoShop.Application.Shared.Exceptions;

public class InnoShopException : ApplicationException {

    public string Title;
    public InnoShopException(string title) => Title = title;
    public InnoShopException(string title, string? message) : base(message) => Title = title;
    public InnoShopException(string title, string? message, Exception? innerException) 
        : base(message, innerException) => Title = title;
}