using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.BalanceAfter)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Remarks)
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        // Index for fast lookup by InventoryId
        builder.HasIndex(x => x.InventoryId)
               .HasDatabaseName("IX_InventoryTransaction_InventoryId");

        // Index for reference lookup
        builder.HasIndex(x => x.ReferenceId)
               .HasDatabaseName("IX_InventoryTransaction_ReferenceId");
    }
}
