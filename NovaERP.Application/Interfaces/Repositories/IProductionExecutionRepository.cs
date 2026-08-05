using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IProductionExecutionRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<ProductionExecution>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<ProductionExecution?> GetByIdAsync(Guid id);
    Task AddAsync(ProductionExecution execution);
    void Update(ProductionExecution execution);
    void Delete(ProductionExecution execution);
}
