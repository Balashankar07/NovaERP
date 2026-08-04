namespace NovaERP.Application.Features.ProductionPlans.DTOs;

public class ProductionRequirementDto
{
    public Guid Id { get; set; }
    public Guid ProductionPlanId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
