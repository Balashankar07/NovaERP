using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.QualityInspections.DTOs;

public class CreateQualityDefectDto
{
    [Required]
    [MaxLength(50)]
    public string DefectCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DefectName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public decimal Quantity { get; set; }

    [MaxLength(50)]
    public string? Severity { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
