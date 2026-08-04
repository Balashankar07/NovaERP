using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Suppliers.DTOs;

public class CreateSupplierDto
{
    [Required]
    [MaxLength(50)]
    public string SupplierCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? TaxRegistrationNumber { get; set; }

    [MaxLength(50)]
    public string? PaymentTerms { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; }

    public decimal? CreditLimit { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}
