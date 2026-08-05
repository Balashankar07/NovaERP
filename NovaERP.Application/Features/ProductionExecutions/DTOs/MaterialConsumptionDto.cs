namespace NovaERP.Application.Features.ProductionExecutions.DTOs;

public class MaterialConsumptionDto
{
    public Guid Id { get; set; }
    public Guid ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    public Guid InventoryId { get; set; }
    
    public decimal RequiredQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
