using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InnoShop.Domain.Abstraction;

namespace InnoShop.Domain.Entities;

[Table("products")]
public class Product : IProduct {
    
    [Key]
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("title")]
    [MaxLength(128)]
    public required string Title { get; set; }

    [Column("price")]
    public required decimal Price { get; set; }

    [Column("desc")]
    [MaxLength(1024)]
    public required string Description { get; set; }

    [Column("status")]
    public required AviabilityStatus Aviability { get; set; }
    
    [Column("user_id")]
    public required string UserId { get; set; }

    [Column("creation_timestamp")]
    public required DateTime CreationTimestamp { get; set; }

    [Column("last_update_timestamp")]
    public required DateTime LastUpdateTimestamp { get; set; }
}
