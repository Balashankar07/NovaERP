namespace NovaERP.Application.Features.Inventory.DTOs;

public class InventoryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? WarehouseLocationId { get; set; }
    public string? LocationName { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal MinimumLevel { get; set; }
    public decimal MaximumLevel { get; set; }
    public DateTime LastStockUpdate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
