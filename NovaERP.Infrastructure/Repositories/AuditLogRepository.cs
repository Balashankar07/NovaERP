using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<NovaERP.Application.Common.Models.PagedResult<NovaERP.Domain.Entities.AuditLog>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Action.Contains(search) || x.EntityName.Contains(search) || x.EntityId.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            query = sortBy.ToLower() switch
            {
                "action" => isDesc ? query.OrderByDescending(x => x.Action) : query.OrderBy(x => x.Action),
                "entityname" => isDesc ? query.OrderByDescending(x => x.EntityName) : query.OrderBy(x => x.EntityName),
                "timestamp" => isDesc ? query.OrderByDescending(x => x.Timestamp) : query.OrderBy(x => x.Timestamp),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
            };
        }

        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new NovaERP.Application.Common.Models.PagedResult<NovaERP.Domain.Entities.AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

}
