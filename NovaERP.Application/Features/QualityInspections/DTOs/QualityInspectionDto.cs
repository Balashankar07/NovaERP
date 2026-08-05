namespace NovaERP.Application.Features.QualityInspections.DTOs;

public class QualityInspectionDto
{
    public Guid Id { get; set; }
    public string InspectionNumber { get; set; } = string.Empty;
    public Guid ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal InspectedQuantity { get; set; }
    public decimal PassedQuantity { get; set; }
    public decimal FailedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? InspectorId { get; set; }
    public DateTime? InspectionDate { get; set; }
    public string? Remarks { get; set; }

    public ICollection<QualityDefectDto> QualityDefects { get; set; } = new List<QualityDefectDto>();
}
