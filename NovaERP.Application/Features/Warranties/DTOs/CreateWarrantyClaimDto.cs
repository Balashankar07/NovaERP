using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Warranties.DTOs;

public class CreateWarrantyClaimDto
{
    [Required]
    public Guid WarrantyId { get; set; }
    
    [Required]
    public string Complaint { get; set; } = string.Empty;
}
