using InnoShop.Application.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Application.Shared.Misc;

public static class ResultExtensions {
    public static IActionResult ToActionResult<T>(this Result<T> result) {
        if (result.IsSuccess) {
            return SuccessValuedResult(result);
        }

        var failedResult = new Result()
            .WithErrors(result.Errors);

        return failedResult.ToActionResult();
    }

    public static IActionResult ToActionResult(this Result result) {
        if (result.IsSuccess) {
            return SuccessResult();
        }

        return ErrorResult(result);
    }

    private static IActionResult SuccessValuedResult<T>(IResult<T> result) {
        return new OkObjectResult(result.ValueOrDefault);
    }

    private static IActionResult SuccessResult() {
        return new OkResult();
    }

    private static IActionResult ErrorResult(IResultBase result) {
        var response = new FailedResult() {
            Status = FailedStatus.Error,
            Message = result.Errors
                .Select(e => e.Message)
                .Aggregate((x, y) => $"{x}\n{y}")
        };

        return new BadRequestObjectResult(response);
    }
}