using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class BOMItem : AuditableEntity
{
    public Guid BomId { get; set; }
    public BOM BOM { get; set; } = null!;

    public Guid RawMaterialProductId { get; set; }
    public Product RawMaterialProduct { get; set; } = null!;

    public decimal Quantity { get; set; }

    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public decimal WastePercentage { get; set; }

    public string? Remarks { get; set; }
}
