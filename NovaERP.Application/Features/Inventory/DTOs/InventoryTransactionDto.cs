namespace NovaERP.Application.Features.Inventory.DTOs;

public class InventoryTransactionDto
{
    public Guid Id { get; set; }
    public Guid InventoryId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Remarks { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
