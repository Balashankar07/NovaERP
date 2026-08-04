using NovaERP.Domain.Entities;
using NovaERP.Application.Common.Models;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetPurchaseOrderWithItemsAsync(Guid id);
    Task<string> GeneratePONumberAsync();
}
