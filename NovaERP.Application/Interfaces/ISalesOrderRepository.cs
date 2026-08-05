using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces;

public interface ISalesOrderRepository : IRepository<SalesOrder>
{
    Task<PagedResult<SalesOrder>> GetSalesOrdersPagedAsync(int pageNumber, int pageSize, string? search, string? sortBy, string? sortOrder);
    Task<SalesOrder?> GetSalesOrderWithDetailsAsync(Guid id);
    Task<string> GenerateOrderNumberAsync();
}
