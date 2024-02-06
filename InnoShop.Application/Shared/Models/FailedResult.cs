namespace InnoShop.Application.Shared.Models;


public enum FailedStatus {
    Invalid,
    Error,
}

public class FailedResult {
    public required FailedStatus Status { get; set; }
    public required string Message { get; set; }
}