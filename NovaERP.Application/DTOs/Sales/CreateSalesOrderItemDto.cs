namespace NovaERP.Application.DTOs.Sales;

public class CreateSalesOrderItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
