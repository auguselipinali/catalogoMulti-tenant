using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Persistence;

// Seed de desarrollo. Passwords de desarrollo: Admin@Lore123! / Admin@Nova123!
// Cambiar por variables de entorno antes de producción.
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await SeedLoreAsync(context, passwordHasher);
        await SeedNovaAsync(context, passwordHasher);
    }

    private static async Task SeedLoreAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Tenants.AnyAsync(t => t.Slug == "lore"))
            return;

        var tenant = Tenant.Create("Lore Perfumería", "lore");
        context.Tenants.Add(tenant);
        context.Users.Add(User.Create("admin@lore.com", passwordHasher.Hash("Admin@Lore123!"), tenant.Id));

        // TenantId set explicitly: seeder runs without HTTP context (ITenantContext.TenantId is null).
        // SaveChangesAsync allows this when TenantId is already set and context is null.
        var p1 = Product.Create("Perfume Floral 50ml", 45.00m, "Fragancia floral intensa");
        p1.TenantId = tenant.Id;
        var p2 = Product.Create("Colonia Citrus 100ml", 62.00m, "Colonia fresca con notas cítricas");
        p2.TenantId = tenant.Id;
        context.Products.Add(p1);
        context.Products.Add(p2);

        await context.SaveChangesAsync();
    }

    private static async Task SeedNovaAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Tenants.AnyAsync(t => t.Slug == "nova"))
            return;

        var tenant = Tenant.Create("Nova Modas", "nova");
        context.Tenants.Add(tenant);
        context.Users.Add(User.Create("admin@nova.com", passwordHasher.Hash("Admin@Nova123!"), tenant.Id));

        var p1 = Product.Create("Remera Básica Blanca", 18.00m, "Remera unisex de algodón");
        p1.TenantId = tenant.Id;
        var p2 = Product.Create("Jean Slim Azul", 55.00m, "Jean slim fit talle standard");
        p2.TenantId = tenant.Id;
        context.Products.Add(p1);
        context.Products.Add(p2);

        await context.SaveChangesAsync();
    }
}
