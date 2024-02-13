using System.Text.Json.Serialization;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Enums;

namespace InnoShop.Application.Shared.Models.Product;

public class ProductResultDto : IProduct {

    [JsonConstructor]
    [Newtonsoft.Json.JsonConstructor]
    public ProductResultDto(
    Guid id,
    string title,
    decimal price,
    string description,
    AvailabilityStatus availability,
    DateTime creationTimestamp,
    DateTime lastUpdateTimestamp,
    string userId,
    bool isEditable) {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Price = price;
        Availability = availability;
        CreationTimestamp = creationTimestamp;
        LastUpdateTimestamp = lastUpdateTimestamp;
        IsEditable = isEditable;
    }

    public ProductResultDto(IProduct other) {
        Id = other.Id;
        UserId = other.UserId;

        Title = other.Title;
        Description = other.Description;
        Price = other.Price;
        Availability = other.Availability;
        CreationTimestamp = other.CreationTimestamp;
        LastUpdateTimestamp = other.LastUpdateTimestamp;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public AvailabilityStatus Availability { get; set; }

    public DateTime CreationTimestamp { get; set; }
    public DateTime LastUpdateTimestamp { get; set; }

    public string UserId { get; set; }

    public required bool IsEditable { get; set; }
}