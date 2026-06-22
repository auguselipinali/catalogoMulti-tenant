using CatalogoMultiTenant.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CatalogoMultiTenant.Infrastructure.Services;

public class TenantContextService : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContextService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public Guid? TenantId
    {
        get
        {
            // JWT claim takes priority (authenticated routes).
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id");
            if (claim is not null && Guid.TryParse(claim.Value, out var jwtId))
                return jwtId;

            // Fallback: slug resolution set by TenantSlugMiddleware (public routes).
            if (_httpContextAccessor.HttpContext?.Items[TenantResolutionKeys.TenantId] is Guid slugId)
                return slugId;

            return null;
        }
    }

    public string? TenantSlug
    {
        get
        {
            var jwtSlug = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_slug")?.Value;
            if (jwtSlug is not null)
                return jwtSlug;

            return _httpContextAccessor.HttpContext?.Items[TenantResolutionKeys.TenantSlug] as string;
        }
    }
}
