using CatalogoMultiTenant.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Middleware;

public class TenantSlugMiddleware
{
    private readonly RequestDelegate _next;

    public TenantSlugMiddleware(RequestDelegate next) => _next = next;

    // IAppDbContext is injected per-request via InvokeAsync (not constructor) because
    // it is Scoped and the middleware itself is a singleton.
    public async Task InvokeAsync(HttpContext context, IAppDbContext db)
    {
        if (context.GetRouteValue("slug") is string slug)
        {
            // Tenant is not a TenantedEntity, so this query is not affected by the
            // global query filter and works correctly with a null ITenantContext.
            var tenant = await db.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.Items[TenantResolutionKeys.TenantId] = tenant.Id;
            context.Items[TenantResolutionKeys.TenantSlug] = tenant.Slug;
        }

        await _next(context);
    }
}
