using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PONumber).IsUnique();
        builder.Property(x => x.PONumber).IsRequired().HasMaxLength(50);
        
        builder.Property(x => x.Currency).HasMaxLength(3);

        builder.HasOne(x => x.Supplier)
               .WithMany()
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);
               
        builder.HasMany(x => x.Items)
               .WithOne(x => x.PurchaseOrder)
               .HasForeignKey(x => x.PurchaseOrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
