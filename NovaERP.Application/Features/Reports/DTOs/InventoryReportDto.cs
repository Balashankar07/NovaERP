namespace NovaERP.Application.Features.Reports.DTOs;

public class InventoryReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public decimal CostPrice { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastRestockDate { get; set; }
}
