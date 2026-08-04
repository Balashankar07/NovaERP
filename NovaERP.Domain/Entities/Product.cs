using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class Product : AuditableEntity
{
    public string ProductCode { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;

    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int MinimumStock { get; set; }

    public int MaximumStock { get; set; }

    public int ReorderLevel { get; set; }

    public string? Barcode { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
