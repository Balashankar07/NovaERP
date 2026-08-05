using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Data.Configurations;

public class WarrantyClaimConfiguration : IEntityTypeConfiguration<WarrantyClaim>
{
    public void Configure(EntityTypeBuilder<WarrantyClaim> builder)
    {
        builder.ToTable("ServiceRequests"); // As specified in DB Schema

        builder.HasKey(wc => wc.Id);

        builder.Property(wc => wc.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(wc => wc.Warranty)
            .WithMany(w => w.WarrantyClaims)
            .HasForeignKey(wc => wc.WarrantyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
