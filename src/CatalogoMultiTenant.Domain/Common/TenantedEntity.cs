namespace CatalogoMultiTenant.Domain.Common;

public abstract class TenantedEntity
{
    public Guid TenantId { get; set; }
}
