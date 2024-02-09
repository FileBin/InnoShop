using InnoShop.Domain.Enums;

namespace InnoShop.Domain.Abstraction;

public interface IBaseSearchQuery {
    public int From { get; }
    public int To { get; }

    public SortingType SortingType { get; }
    public SortingOrder SortingOrder { get; }
}
