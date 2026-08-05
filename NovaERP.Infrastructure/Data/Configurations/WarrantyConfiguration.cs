using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Data.Configurations;

public class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
{
    public void Configure(EntityTypeBuilder<Warranty> builder)
    {
        builder.ToTable("Warranties");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(w => w.SerialNumber)
            .IsUnique();

        builder.Property(w => w.WarrantyType)
            .HasMaxLength(50);

        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(w => w.Product)
            .WithMany()
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(w => w.Shipment)
            .WithMany()
            .HasForeignKey(w => w.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
