using System.Dynamic;

namespace InnoShop.Application.Shared.Auth;

public class BaseResult {
    public required bool IsSuccessful { get; set; }
    public string? Message { get; set; }

    public static BaseResult Error(string message)
        => new BaseResult { IsSuccessful = false, Message = message };


    public static BaseResult Success(string? message = null) => new BaseResult { IsSuccessful = true, Message = message };
}
