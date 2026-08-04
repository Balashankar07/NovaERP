using NovaERP.Application.Features.Products.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IProductService
{
    Task<NovaERP.Application.Common.Models.PagedResult<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<ProductDto?> GetByIdAsync(Guid id);

    Task<ProductDto> CreateAsync(CreateProductDto dto);

    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);

    Task<bool> DeleteAsync(Guid id);
}
