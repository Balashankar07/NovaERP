using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.ProductionExecutions.DTOs;

public class CreateProductionExecutionDto
{
    [Required]
    public Guid ProductionOrderId { get; set; }
    
    public string? Remarks { get; set; }
}
