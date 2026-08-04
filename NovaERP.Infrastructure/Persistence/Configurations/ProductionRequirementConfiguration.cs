using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class ProductionRequirementConfiguration : IEntityTypeConfiguration<ProductionRequirement>
{
    public void Configure(EntityTypeBuilder<ProductionRequirement> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequiredQuantity)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.AvailableQuantity)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.ShortageQuantity)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Unit)
               .WithMany()
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}
