using NovaERP.Application.Features.ProductCategories.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IProductCategoryService
{
    Task<NovaERP.Application.Common.Models.PagedResult<ProductCategoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<ProductCategoryDto?> GetByIdAsync(Guid id);

    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto dto);

    Task<ProductCategoryDto?> UpdateAsync(Guid id, UpdateProductCategoryDto dto);

    Task<bool> DeleteAsync(Guid id);
}
