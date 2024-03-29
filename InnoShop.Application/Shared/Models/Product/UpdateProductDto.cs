using System.Text.Json.Serialization;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Enums;

namespace InnoShop.Application.Shared.Models.Product;

public class UpdateProductDto : IProductDescriptorNullable {

    [JsonConstructor]
    public UpdateProductDto(string? title = null, string? description = null, decimal? price = null, AvailabilityStatus? status = null) {
        Title = title;
        Price = price;
        Description = description;
        Status = status;
    }

    public UpdateProductDto(UpdateProductDto other) {
        Title = other.Title;
        Price = other.Price;
        Description = other.Description;
        Status = other.Status;
    }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public AvailabilityStatus? Status { get; set; }
}
