using System.ComponentModel.DataAnnotations.Schema;
using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class GoodsReceiptItem : AuditableEntity
{
    public Guid GoodsReceiptId { get; set; }
    public GoodsReceipt? GoodsReceipt { get; set; }
    
    public Guid PurchaseOrderItemId { get; set; }
    public PurchaseOrderItem? PurchaseOrderItem { get; set; }
    
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OrderedQuantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ReceivedQuantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal RejectedQuantity { get; set; }
    
    public string? Remarks { get; set; }
}
