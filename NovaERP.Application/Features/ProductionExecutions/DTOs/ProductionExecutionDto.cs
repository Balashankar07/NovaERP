using NovaERP.Domain.Enums;

namespace NovaERP.Application.Features.ProductionExecutions.DTOs;

public class ProductionExecutionDto
{
    public Guid Id { get; set; }
    public string ExecutionNumber { get; set; } = string.Empty;
    public Guid ProductionOrderId { get; set; }
    
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public decimal ProducedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    
    public ProductionExecutionStatus Status { get; set; }
    public string? Remarks { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}
