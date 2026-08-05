using NovaERP.Domain.Common;
using NovaERP.Domain.Enums;

namespace NovaERP.Domain.Entities;

public class WarrantyClaim : AuditableEntity
{
    public Guid WarrantyId { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string? Resolution { get; set; }
    public WarrantyClaimStatus Status { get; set; } = WarrantyClaimStatus.Pending;

    // Navigation Properties
    public Warranty? Warranty { get; set; }
}
