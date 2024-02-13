namespace InnoShop.Application.Shared.Models.Product;

public class SearchResultDto {
    public required IEnumerable<ProductResultDto> Products { get; set; }
    public required int QueryCount { get; set; }
}
