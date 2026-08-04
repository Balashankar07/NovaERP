using NovaERP.Application.Features.ProductCategories.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ProductCategoryService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<ProductCategoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var categories = await _unitOfWork.ProductCategories.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);

        return new NovaERP.Application.Common.Models.PagedResult<ProductCategoryDto>
        {
            Items = categories.Items.Select(MapToDto).ToList(),
            TotalCount = categories.TotalCount,
            PageNumber = categories.PageNumber,
            PageSize = categories.PageSize
        };
    }

    public async Task<ProductCategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);
        if (category == null) return null;
        return MapToDto(category);
    }

    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto dto)
    {
        var category = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ProductCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Create", "ProductCategory", category.Id.ToString(), newValues: $"Name: {category.Name}");

        return MapToDto(category);
    }

    public async Task<ProductCategoryDto?> UpdateAsync(Guid id, UpdateProductCategoryDto dto)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);
        if (category == null) return null;

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ProductCategories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Update", "ProductCategory", category.Id.ToString());

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);
        if (category == null) return false;

        await _unitOfWork.ProductCategories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "ProductCategory", category.Id.ToString());

        return true;
    }

    private static ProductCategoryDto MapToDto(ProductCategory category)
    {
        return new ProductCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}
