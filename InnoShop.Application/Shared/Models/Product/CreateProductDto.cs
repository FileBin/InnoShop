using System.Text.Json.Serialization;
using InnoShop.Domain.Abstraction;

namespace InnoShop.Application.Shared.Models.Product;

public class CreateProductDto : IProductDescriptor {
    [JsonConstructor]
    public CreateProductDto(string title, string description, decimal price) {
        Title = title;
        Price = price;
        Description = description;
    }

    public CreateProductDto(CreateProductDto other) {
        Title = other.Title;
        Price = other.Price;
        Description = other.Description;
    }
    
    public string Title { get; set; }
    public string Description { get; set; }

    public decimal Price { get; set; }
}
