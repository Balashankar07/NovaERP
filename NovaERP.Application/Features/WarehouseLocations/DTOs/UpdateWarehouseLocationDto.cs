using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.WarehouseLocations.DTOs;

public class UpdateWarehouseLocationDto
{
    [Required]
    [MaxLength(100)]
    public string LocationName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Zone { get; set; }

    [MaxLength(50)]
    public string? Rack { get; set; }

    [MaxLength(50)]
    public string? Shelf { get; set; }

    [MaxLength(50)]
    public string? Bin { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
