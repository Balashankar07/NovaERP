using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.GoodsReceipts.DTOs;

public class UpdateGoodsReceiptDto
{
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<UpdateGoodsReceiptItemDto> Items { get; set; } = new();
}
