using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Persistence;

// Seed de desarrollo: crea el primer tenant y su usuario admin si no existen.
// Password de desarrollo: Admin@Lore123!
// Cambiar por variables de entorno antes de producción.
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Tenants.AnyAsync(t => t.Slug == "lore"))
            return;

        var tenant = Tenant.Create("Lore Perfumería", "lore");
        context.Tenants.Add(tenant);

        var adminUser = User.Create(
            email: "admin@lore.com",
            passwordHash: passwordHasher.Hash("Admin@Lore123!"),
            tenantId: tenant.Id);

        context.Users.Add(adminUser);

        await context.SaveChangesAsync();
    }
}
