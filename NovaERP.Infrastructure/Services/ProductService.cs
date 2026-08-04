using NovaERP.Application.Features.Products.DTOs;
using NovaERP.Application.Features.ProductCategories.DTOs;
using NovaERP.Application.Features.Brands.DTOs;
using NovaERP.Application.Features.Units.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ProductService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var products = await _unitOfWork.Products.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);

        return new NovaERP.Application.Common.Models.PagedResult<ProductDto>
        {
            Items = products.Items.Select(MapToDto).ToList(),
            TotalCount = products.TotalCount,
            PageNumber = products.PageNumber,
            PageSize = products.PageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return null;
        return MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var existingCode = await _unitOfWork.Products.GetByCodeAsync(dto.ProductCode);
        if (existingCode != null)
            throw new Exception("Product code already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductCode = dto.ProductCode,
            SKU = dto.SKU,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            UnitId = dto.UnitId,
            CostPrice = dto.CostPrice,
            SellingPrice = dto.SellingPrice,
            MinimumStock = dto.MinimumStock,
            MaximumStock = dto.MaximumStock,
            ReorderLevel = dto.ReorderLevel,
            Barcode = dto.Barcode,
            ImageUrl = dto.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Create", "Product", product.Id.ToString(), newValues: $"Code: {product.ProductCode}, Name: {product.Name}");

        // Refetch to include relationships for mapping
        var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
        return MapToDto(createdProduct!);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return null;

        if (product.ProductCode != dto.ProductCode)
        {
            var existingCode = await _unitOfWork.Products.GetByCodeAsync(dto.ProductCode);
            if (existingCode != null)
                throw new Exception("Product code already exists.");
        }

        product.ProductCode = dto.ProductCode;
        product.SKU = dto.SKU;
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.UnitId = dto.UnitId;
        product.CostPrice = dto.CostPrice;
        product.SellingPrice = dto.SellingPrice;
        product.MinimumStock = dto.MinimumStock;
        product.MaximumStock = dto.MaximumStock;
        product.ReorderLevel = dto.ReorderLevel;
        product.Barcode = dto.Barcode;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Update", "Product", product.Id.ToString());

        var updatedProduct = await _unitOfWork.Products.GetByIdAsync(id);
        return MapToDto(updatedProduct!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return false;

        await _unitOfWork.Products.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "Product", product.Id.ToString());

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Category = product.Category != null ? new ProductCategoryDto 
            { 
                Id = product.Category.Id, 
                Name = product.Category.Name, 
                Description = product.Category.Description, 
                IsActive = product.Category.IsActive 
            } : null,
            BrandId = product.BrandId,
            Brand = product.Brand != null ? new BrandDto 
            { 
                Id = product.Brand.Id, 
                Name = product.Brand.Name, 
                Description = product.Brand.Description, 
                IsActive = product.Brand.IsActive 
            } : null,
            UnitId = product.UnitId,
            Unit = product.Unit != null ? new UnitDto 
            { 
                Id = product.Unit.Id, 
                Name = product.Unit.Name, 
                Abbreviation = product.Unit.Abbreviation, 
                Description = product.Unit.Description, 
                IsActive = product.Unit.IsActive 
            } : null,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            MinimumStock = product.MinimumStock,
            MaximumStock = product.MaximumStock,
            ReorderLevel = product.ReorderLevel,
            Barcode = product.Barcode,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };
    }
}
