using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Warehouses.DTOs;

public class CreateWarehouseDto
{
    [Required]
    [MaxLength(50)]
    public string WarehouseCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string WarehouseName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? ManagerName { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    public bool IsDefault { get; set; }
}
