namespace CatalogoMultiTenant.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private Tenant() { }

    public static Tenant Create(string name, string slug)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
