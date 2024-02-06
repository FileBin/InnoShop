using InnoShop.Application.Shared.Interfaces;

namespace InnoShop.Application.Validation;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IBaseCommand {
    private readonly IEnumerable<IValidator<TRequest>> validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => this.validators = validators;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        if (!validators.Any()) {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);
        var errors = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToArray();
        if (errors.Length > 0) {
            throw new ValidationException(errors);
        }
        return await next();
    }
}