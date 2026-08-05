using NovaERP.Domain.Common;
using NovaERP.Domain.Enums;

namespace NovaERP.Domain.Entities;

public class QualityInspection : AuditableEntity
{
    public string InspectionNumber { get; set; } = string.Empty;
    public Guid ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    
    public decimal InspectedQuantity { get; set; }
    public decimal PassedQuantity { get; set; }
    public decimal FailedQuantity { get; set; }
    
    public QualityInspectionStatus Status { get; set; } = QualityInspectionStatus.Draft;
    
    public Guid? InspectorId { get; set; }
    public DateTime? InspectionDate { get; set; }
    
    public string? Remarks { get; set; }

    public ProductionExecution? ProductionExecution { get; set; }
    public Product? Product { get; set; }
    public User? Inspector { get; set; }
    
    public ICollection<QualityDefect> QualityDefects { get; set; } = new List<QualityDefect>();
}
