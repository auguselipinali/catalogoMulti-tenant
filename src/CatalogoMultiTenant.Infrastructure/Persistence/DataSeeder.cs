using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Infrastructure.Persistence;

// Seed de datos. El tenant esencial (Caricias al Alma + admin) se siembra en TODOS
// los entornos, incluido Production, de forma idempotente (guard por slug). La
// password del admin viene de SEED_ADMIN_PASSWORD (fallback al valor de dev).
// Nova Modas es data de demo y solo se siembra cuando includeDemoData = true
// (Development), para no ensuciar la base productiva.
public static class DataSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        string adminPassword,
        bool includeDemoData)
    {
        await SeedCariciasAlAlmaAsync(context, passwordHasher, adminPassword);

        if (includeDemoData)
            await SeedNovaAsync(context, passwordHasher);
    }

    private static async Task SeedCariciasAlAlmaAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        string adminPassword)
    {
        if (await context.Tenants.AnyAsync(t => t.Slug == "caricias-al-alma"))
            return;

        var tenant = Tenant.Create("Caricias al Alma", "caricias-al-alma");
        context.Tenants.Add(tenant);
        context.Users.Add(User.Create("admin@lore.com", passwordHasher.Hash(adminPassword), tenant.Id));

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
