using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<Product?> GetByIdAsync(Guid id);

    Task<Product?> GetByCodeAsync(string code);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Product product);
}
