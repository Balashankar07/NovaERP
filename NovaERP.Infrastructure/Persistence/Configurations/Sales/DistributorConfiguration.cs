using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations.Sales;

public class DistributorConfiguration : IEntityTypeConfiguration<Distributor>
{
    public void Configure(EntityTypeBuilder<Distributor> builder)
    {
        builder.ToTable("Distributors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.CompanyName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.ContactPerson)
            .HasMaxLength(100);

        builder.Property(d => d.Phone)
            .HasMaxLength(20);

        builder.Property(d => d.Email)
            .HasMaxLength(150);

        builder.Property(d => d.Address)
            .HasColumnType("TEXT");

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);
    }
}
