using System.ComponentModel.DataAnnotations.Schema;
using NovaERP.Domain.Common;
using NovaERP.Domain.Enums;

namespace NovaERP.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid InventoryId { get; set; }
    public Inventory? Inventory { get; set; }

    public InventoryTransactionType TransactionType { get; set; }

    public InventoryReferenceType ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }

    public string? Remarks { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}
