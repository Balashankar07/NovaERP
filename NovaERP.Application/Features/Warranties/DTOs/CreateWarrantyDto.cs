using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Warranties.DTOs;

public class CreateWarrantyDto
{
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public Guid ShipmentId { get; set; }
    [Required]
    public string SerialNumber { get; set; } = string.Empty;
    [Required]
    public string WarrantyType { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}
