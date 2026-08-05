using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IMaterialConsumptionRepository
{
    Task<List<MaterialConsumption>> GetByProductionExecutionIdAsync(Guid executionId);
    Task AddAsync(MaterialConsumption consumption);
    Task AddRangeAsync(IEnumerable<MaterialConsumption> consumptions);
}
