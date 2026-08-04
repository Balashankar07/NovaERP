using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class ProductionRequirement : BaseEntity
{
    public Guid ProductionPlanId { get; set; }
    public ProductionPlan ProductionPlan { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
