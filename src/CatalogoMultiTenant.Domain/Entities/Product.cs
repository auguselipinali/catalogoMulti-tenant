using CatalogoMultiTenant.Domain.Common;

namespace CatalogoMultiTenant.Domain.Entities;

public class Product : TenantedEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }

    private Product() { }

    public static Product Create(string name, decimal price, string? description = null, string? imageUrl = null)
        => new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Description = description,
            ImageUrl = imageUrl
        };
}
