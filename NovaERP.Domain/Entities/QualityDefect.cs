using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class QualityDefect : AuditableEntity
{
    public Guid QualityInspectionId { get; set; }
    
    public string DefectCode { get; set; } = string.Empty;
    public string DefectName { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    
    public string? Severity { get; set; }
    public string? Remarks { get; set; }

    public QualityInspection? QualityInspection { get; set; }
}
