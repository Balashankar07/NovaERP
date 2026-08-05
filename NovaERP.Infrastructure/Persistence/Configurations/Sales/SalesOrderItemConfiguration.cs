using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations.Sales;

public class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.ToTable("SalesOrderItems");

        builder.HasKey(soi => soi.Id);

        builder.Property(soi => soi.Quantity)
            .HasPrecision(18, 2);

        builder.Property(soi => soi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(soi => soi.TotalPrice)
            .HasPrecision(18, 2);

        builder.HasOne(soi => soi.Product)
            .WithMany()
            .HasForeignKey(soi => soi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
