using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.PurchaseOrders.DTOs;

public class UpdatePurchaseOrderDto
{
    [Required]
    public Guid SupplierId { get; set; }
    
    [Required]
    public DateTime ExpectedDeliveryDate { get; set; }
    
    public string Currency { get; set; } = "USD";
    
    public string? Remarks { get; set; }
    
    public bool IsActive { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new();
}
