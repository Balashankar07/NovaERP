using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IBOMRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<BOM>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<BOM?> GetByIdAsync(Guid id);
    Task<BOM?> GetActiveByProductIdAsync(Guid productId);
    Task AddAsync(BOM bom);
    Task UpdateAsync(BOM bom);
    Task DeleteAsync(BOM bom);
}
