using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using CatalogoMultiTenant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
