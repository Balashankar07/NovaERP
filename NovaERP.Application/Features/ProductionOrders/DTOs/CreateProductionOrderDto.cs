using System.ComponentModel.DataAnnotations;
using NovaERP.Domain.Enums;

namespace NovaERP.Application.Features.ProductionOrders.DTOs;

public class CreateProductionOrderDto
{
    [Required]
    public Guid ProductionPlanId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "PlannedQuantity must be greater than zero.")]
    public decimal PlannedQuantity { get; set; }

    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }

    public string? WorkCenter { get; set; }
    public string? Supervisor { get; set; }

    public ProductionOrderPriority Priority { get; set; } = ProductionOrderPriority.Medium;

    public string? Remarks { get; set; }
}
