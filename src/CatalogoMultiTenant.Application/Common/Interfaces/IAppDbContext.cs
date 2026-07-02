using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
