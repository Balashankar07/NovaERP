using NovaERP.Domain.Enums;

namespace NovaERP.Application.Features.Warranties.DTOs;

public class UpdateWarrantyClaimDto
{
    public WarrantyClaimStatus? Status { get; set; }
    public string? Resolution { get; set; }
}
