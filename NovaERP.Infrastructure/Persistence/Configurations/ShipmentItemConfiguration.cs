using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.HasKey(si => si.Id);

        builder.Property(si => si.Quantity)
            .HasColumnType("decimal(18,2)");

        builder.Property(si => si.DeliveredQuantity)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
