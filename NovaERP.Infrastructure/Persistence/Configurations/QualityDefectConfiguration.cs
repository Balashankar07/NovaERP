using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class QualityDefectConfiguration : IEntityTypeConfiguration<QualityDefect>
{
    public void Configure(EntityTypeBuilder<QualityDefect> builder)
    {
        builder.ToTable("QualityDefects");

        builder.HasKey(qd => qd.Id);

        builder.Property(qd => qd.DefectCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(qd => qd.DefectName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(qd => qd.Quantity)
            .HasPrecision(18, 2);

        builder.Property(qd => qd.Severity)
            .HasMaxLength(50);

        builder.Property(qd => qd.Remarks)
            .HasMaxLength(500);
    }
}
