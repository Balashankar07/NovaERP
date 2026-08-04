using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<Brand>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _context.Brands.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            query = sortBy.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "createdat" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
            };
        }

        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new NovaERP.Application.Common.Models.PagedResult<Brand>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Brand?> GetByIdAsync(Guid id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public Task AddAsync(Brand brand)
    {
        _context.Brands.AddAsync(brand);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Brand brand)
    {
        _context.Brands.Remove(brand);
        return Task.CompletedTask;
    }
}
