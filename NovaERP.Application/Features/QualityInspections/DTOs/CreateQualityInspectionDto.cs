using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.QualityInspections.DTOs;

public class CreateQualityInspectionDto
{
    [Required]
    public Guid ProductionExecutionId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Inspected Quantity must be greater than zero.")]
    public decimal InspectedQuantity { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
