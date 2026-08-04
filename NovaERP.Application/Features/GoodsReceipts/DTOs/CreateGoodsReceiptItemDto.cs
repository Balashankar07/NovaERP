using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.GoodsReceipts.DTOs;

public class CreateGoodsReceiptItemDto
{
    [Required]
    public Guid PurchaseOrderItemId { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Received quantity must be non-negative.")]
    public decimal ReceivedQuantity { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Rejected quantity must be non-negative.")]
    public decimal RejectedQuantity { get; set; }
    
    public string? Remarks { get; set; }
}
