using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.WarehouseLocations.DTOs;

public class CreateWarehouseLocationDto
{
    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [MaxLength(50)]
    public string LocationCode { get; set; } = string.Empty;

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
}
