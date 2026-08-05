using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class MaterialConsumption : AuditableEntity
{
    public Guid ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    public Guid InventoryId { get; set; }
    
    public decimal RequiredQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; } // Can be Required - Consumed, or vice versa, typically expected - actual

    public ProductionExecution? ProductionExecution { get; set; }
    public Product? Product { get; set; }
    public Inventory? Inventory { get; set; }
}
