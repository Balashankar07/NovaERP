using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(s => s.CourierName)
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(s => s.SalesOrder)
            .WithMany()
            .HasForeignKey(s => s.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.ShipmentItems)
            .WithOne(si => si.Shipment)
            .HasForeignKey(si => si.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
