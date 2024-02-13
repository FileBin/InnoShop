using InnoShop.Application.Shared.Interfaces;

using InnoShop.Application.Validation;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain.Abstraction;
using InnoShop.Application.Shared.Models.Product;

namespace InnoShop.Application.Shared.Commands.Products;

public class SearchProductsCommand : SearchQueryDto, ICommand<SearchResultDto> {
    public SearchProductsCommand(SearchQueryDto other) : base(other) { }

    public required IUserDescriptor UserDesc { get; set; }
}

public sealed class SearchProductsValidator : AbstractValidator<SearchProductsCommand> {
    public SearchProductsValidator() {
        RuleFor(x => (long)x.To - x.From).GreaterThanOrEqualTo(1).LessThanOrEqualTo(Util.MaxQuery);

        When(x => x.PriceFrom is not null, () => {
            RuleFor(x => x.PriceFrom!.Value).PriceValidation();
        });

        When(x => x.PriceUpTo is not null, () => {
            RuleFor(x => x.PriceUpTo!.Value).PriceValidation();
            When(x => x.PriceFrom is not null, () => {
                RuleFor(x => x.PriceUpTo!.Value - x.PriceFrom!.Value).GreaterThanOrEqualTo(0);
            });
        });

        When(x => x.Contains is not null, () => {
            RuleFor(x => x.Contains!).SearchStringValidation();
        });

        RuleFor(x => x.SortingOrder).IsInEnum();
        RuleFor(x => x.SortingType).IsInEnum();
    }
}

public class SearchProductsCommandHandler(IProductDbContext context)
 : IProductCommandHandler<SearchProductsCommand, SearchResultDto> {
    public async Task<SearchResultDto> Handle(SearchProductsCommand request, CancellationToken cancellationToken) {
        return await context.Products.SearchProducts(request, request.UserDesc, cancellationToken);
    }
}

