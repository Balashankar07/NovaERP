using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.DTOs.Sales;

public class UpdateSalesOrderDto
{
    [Required]
    public Guid DistributorId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required in the sales order.")]
    public List<CreateSalesOrderItemDto> Items { get; set; } = new List<CreateSalesOrderItemDto>();
}
