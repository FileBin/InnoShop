namespace InnoShop.Application.Shared.Models.Product;

using InnoShop.Domain.Entities;

public class SearchResultDto {
    public required IEnumerable<Product> Products { get; set; }
    public required int QueryCount { get; set; }
}
