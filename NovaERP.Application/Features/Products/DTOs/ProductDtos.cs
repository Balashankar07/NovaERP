using NovaERP.Application.Features.Brands.DTOs;
using NovaERP.Application.Features.ProductCategories.DTOs;
using NovaERP.Application.Features.Units.DTOs;

namespace NovaERP.Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Guid CategoryId { get; set; }
    public ProductCategoryDto? Category { get; set; }
    
    public Guid BrandId { get; set; }
    public BrandDto? Brand { get; set; }
    
    public Guid UnitId { get; set; }
    public UnitDto? Unit { get; set; }
    
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public int ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    public Guid UnitId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public int ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateProductDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    public Guid UnitId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public int ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
