using NovaERP.Application.Common.Models;
using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IProductionPlanRepository : IRepository<ProductionPlan>
{
    Task<PagedResult<ProductionPlan>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<ProductionPlan?> GetWithRequirementsAsync(Guid id);
    Task<string> GeneratePlanNumberAsync();
}
