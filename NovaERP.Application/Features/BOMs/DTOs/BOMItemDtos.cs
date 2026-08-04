namespace NovaERP.Application.Features.BOMs.DTOs;

public class BOMItemDto
{
    public Guid Id { get; set; }
    public Guid BomId { get; set; }
    public Guid RawMaterialProductId { get; set; }
    public string RawMaterialProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal WastePercentage { get; set; }
    public string? Remarks { get; set; }
}

public class CreateBOMItemDto
{
    public Guid RawMaterialProductId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public decimal WastePercentage { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateBOMItemDto
{
    public Guid? Id { get; set; } // If null, it's a new item added during update
    public Guid RawMaterialProductId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public decimal WastePercentage { get; set; }
    public string? Remarks { get; set; }
}
