using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly AppDbContext _context;

    public UnitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<Unit>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _context.Units.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || x.Abbreviation.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            query = sortBy.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "abbreviation" => isDesc ? query.OrderByDescending(x => x.Abbreviation) : query.OrderBy(x => x.Abbreviation),
                "createdat" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
            };
        }

        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new NovaERP.Application.Common.Models.PagedResult<Unit>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Unit?> GetByIdAsync(Guid id)
    {
        return await _context.Units.FindAsync(id);
    }

    public Task AddAsync(Unit unit)
    {
        _context.Units.AddAsync(unit);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Unit unit)
    {
        _context.Units.Update(unit);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Unit unit)
    {
        _context.Units.Remove(unit);
        return Task.CompletedTask;
    }
}
