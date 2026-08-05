using NovaERP.Domain.Enums;

namespace NovaERP.Application.Features.Warranties.DTOs;

public class WarrantyClaimDto
{
    public Guid Id { get; set; }
    public Guid WarrantyId { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string? Resolution { get; set; }
    public WarrantyClaimStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
