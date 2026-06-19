using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
