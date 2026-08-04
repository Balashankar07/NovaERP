using NovaERP.Application.Common.Models;
using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
{
    Task<PagedResult<InventoryTransaction>> GetByInventoryIdAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10);
    Task AddTransactionAsync(InventoryTransaction transaction);
}
