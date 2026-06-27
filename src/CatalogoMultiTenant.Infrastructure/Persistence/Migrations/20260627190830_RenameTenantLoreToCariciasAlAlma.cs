using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoMultiTenant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTenantLoreToCariciasAlAlma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Data migration: renames the existing tenant in place (same Id, same products).
            // Name is updated alongside the slug so an existing DB matches a freshly seeded one.
            migrationBuilder.Sql(
                "UPDATE \"Tenants\" SET \"Slug\" = 'caricias-al-alma', \"Name\" = 'Caricias al Alma' WHERE \"Slug\" = 'lore';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Tenants\" SET \"Slug\" = 'lore', \"Name\" = 'Lore Perfumería' WHERE \"Slug\" = 'caricias-al-alma';");
        }
    }
}
