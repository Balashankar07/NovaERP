using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.ProductCode)
            .IsUnique();

        builder.Property(x => x.SKU)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.SKU)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.CostPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.SellingPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Barcode)
            .HasMaxLength(100);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);
    }
}
