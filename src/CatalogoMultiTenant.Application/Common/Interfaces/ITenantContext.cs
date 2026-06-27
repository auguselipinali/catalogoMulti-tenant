namespace CatalogoMultiTenant.Application.Common.Interfaces;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
}
