using NovaERP.Domain.Enums;

namespace NovaERP.Application.DTOs.Sales;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid DistributorId { get; set; }
    public string DistributorName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    
    public List<SalesOrderItemDto> Items { get; set; } = new List<SalesOrderItemDto>();
}
