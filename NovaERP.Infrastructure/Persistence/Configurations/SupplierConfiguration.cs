using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.SupplierCode).IsUnique();

        builder.Property(x => x.SupplierCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SupplierName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(100);

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Mobile)
            .HasMaxLength(20);

        builder.Property(x => x.Website)
            .HasMaxLength(200);

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(200);

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(200);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.State)
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(20);

        builder.Property(x => x.TaxRegistrationNumber)
            .HasMaxLength(50);

        builder.Property(x => x.PaymentTerms)
            .HasMaxLength(50);

        builder.Property(x => x.Currency)
            .HasMaxLength(10);
            
        builder.Property(x => x.CreditLimit)
            .HasColumnType("decimal(18,2)");
    }
}
