namespace CatalogoMultiTenant.Domain.Entities;

// User NO hereda de TenantedEntity y queda explícitamente fuera del global query
// filter (Incremento 4). Razón: el login resuelve al usuario por Email ANTES de
// conocer el tenant; el TenantId del JWT sale del User encontrado. Si User estuviera
// filtrado por tenant, el login nunca podría ejecutarse sin conocer el tenant primero.
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Guid TenantId { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, Guid tenantId)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            TenantId = tenantId
        };
    }
}
