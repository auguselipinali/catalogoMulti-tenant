using CatalogoMultiTenant.Domain.Common;

namespace CatalogoMultiTenant.Domain.Entities;

public class Category : TenantedEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Category() { }

    public static Category Create(string name)
        => new Category
        {
            Id = Guid.NewGuid(),
            Name = name
        };

    public void Rename(string name) => Name = name;
}
