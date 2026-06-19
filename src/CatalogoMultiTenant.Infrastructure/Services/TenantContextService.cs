using System.Security.Claims;
using CatalogoMultiTenant.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CatalogoMultiTenant.Infrastructure.Services;

// Deuda técnica (Incremento 3): sin JWT los claims tenant_id y tenant_slug no
// existen, por lo que TenantId y TenantSlug devuelven null. Cuando Incremento 3
// agregue la generación del JWT con esos claims, este servicio funciona sin cambios.
public class TenantContextService : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id");
            return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public string? TenantSlug
        => _httpContextAccessor.HttpContext?.User.FindFirst("tenant_slug")?.Value;
}
