namespace InnoShop.Domain.Abstraction;

public interface ISearchQuery : IBaseSearchQuery {
    string? Contains { get; }

    decimal? PriceFrom { get; }

    decimal? PriceUpTo { get; }
}