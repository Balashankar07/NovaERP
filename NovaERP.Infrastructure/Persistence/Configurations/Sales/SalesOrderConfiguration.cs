using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations.Sales;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(so => so.Id);

        builder.Property(so => so.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(so => so.OrderNumber)
            .IsUnique();

        builder.Property(so => so.OrderDate)
            .HasColumnType("DATE");

        builder.Property(so => so.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(so => so.Distributor)
            .WithMany(d => d.SalesOrders)
            .HasForeignKey(so => so.DistributorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(so => so.SalesOrderItems)
            .WithOne(i => i.SalesOrder)
            .HasForeignKey(i => i.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
