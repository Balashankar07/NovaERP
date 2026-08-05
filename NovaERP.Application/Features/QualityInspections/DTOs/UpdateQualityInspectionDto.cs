using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.QualityInspections.DTOs;

public class UpdateQualityInspectionDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Passed Quantity cannot be negative.")]
    public decimal PassedQuantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Failed Quantity cannot be negative.")]
    public decimal FailedQuantity { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
