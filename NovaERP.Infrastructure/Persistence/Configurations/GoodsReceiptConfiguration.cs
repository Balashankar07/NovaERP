using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.GRNNumber).IsUnique();
        builder.Property(x => x.GRNNumber).IsRequired().HasMaxLength(50);
        
        builder.HasOne(x => x.PurchaseOrder)
               .WithMany()
               .HasForeignKey(x => x.PurchaseOrderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Supplier)
               .WithMany()
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);
               
        builder.HasMany(x => x.Items)
               .WithOne(x => x.GoodsReceipt)
               .HasForeignKey(x => x.GoodsReceiptId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
