using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IProductCategoryRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<ProductCategory>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<ProductCategory?> GetByIdAsync(Guid id);

    Task AddAsync(ProductCategory category);

    Task UpdateAsync(ProductCategory category);

    Task DeleteAsync(ProductCategory category);
}
