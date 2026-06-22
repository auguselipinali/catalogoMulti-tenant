using CatalogoMultiTenant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogoMultiTenant.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Price).HasColumnType("numeric(18,2)");
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.ImageUrl).HasMaxLength(1000);
    }
}
