using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Application;

public class SearchQueryDto : ISearchQuery {

    public SearchQueryDto() {
        SortingType = SortingType.Descending;
        SortingOrder = SortingOrder.ByDate;
    }

    public SearchQueryDto(SearchQueryDto other) {
        Contains = other.Contains;
        PriceFrom = other.PriceFrom;
        PriceUpTo = other.PriceUpTo;

        From = other.From;
        To = other.To;

        SortingType = other.SortingType;
        SortingOrder = other.SortingOrder;
    }
    [BindProperty(Name = "contains")]
    public string? Contains { get; set; }

    [BindProperty(Name = "price_from")]
    public decimal? PriceFrom { get; set; }

    [BindProperty(Name = "price_to")]
    public decimal? PriceUpTo { get; set; }

    [BindProperty(Name = "from")]
    public int From { get; set; }

    [BindProperty(Name = "to")]
    public int To { get; set; }

    [BindProperty(Name = "type")]
    public SortingType SortingType { get; set; }
    
    [BindProperty(Name = "order")]
    public SortingOrder SortingOrder { get; set; }
}
