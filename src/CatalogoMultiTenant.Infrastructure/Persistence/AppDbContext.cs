using System.Reflection;
using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Common;
using CatalogoMultiTenant.Domain.Entities;
using CatalogoMultiTenant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ITenantContext _tenantContext;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    // Reflection used once at model-build time to apply the global filter to all
    // TenantedEntity subtypes automatically. Documented exception to no-reflection rule.
    private static readonly MethodInfo ApplyTenantFilterMethod =
        typeof(AppDbContext).GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantedEntity).IsAssignableFrom(entityType.ClrType)
                && entityType.ClrType != typeof(TenantedEntity))
            {
                ApplyTenantFilterMethod
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, [modelBuilder]);
            }
        }
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : TenantedEntity
    {
        // When TenantId is null (no resolved tenant), HasValue is false → zero rows returned.
        // Safe default: no context means no data, not all data.
        modelBuilder.Entity<TEntity>().HasQueryFilter(
            e => _tenantContext.TenantId.HasValue && e.TenantId == _tenantContext.TenantId!.Value);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<TenantedEntity>()
                     .Where(e => e.State == EntityState.Added))
        {
            if (_tenantContext.TenantId.HasValue)
            {
                // Normal request: always set from context (prevents accidental cross-tenant writes).
                entry.Entity.TenantId = _tenantContext.TenantId.Value;
            }
            else if (entry.Entity.TenantId == Guid.Empty)
            {
                // No context and no explicit TenantId set: developer forgot to assign it.
                throw new InvalidOperationException(
                    "Cannot persist a TenantedEntity without a resolved TenantId in ITenantContext.");
            }
            // Context null + TenantId already set explicitly (e.g., seeder): allowed through.
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
