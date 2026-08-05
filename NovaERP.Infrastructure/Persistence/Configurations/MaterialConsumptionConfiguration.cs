using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class MaterialConsumptionConfiguration : IEntityTypeConfiguration<MaterialConsumption>
{
    public void Configure(EntityTypeBuilder<MaterialConsumption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequiredQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ConsumedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.VarianceQuantity)
            .HasPrecision(18, 4);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(pe => pe.MaterialConsumptions)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Inventory)
            .WithMany()
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
