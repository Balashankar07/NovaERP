using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _context;

    public ProductCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<ProductCategory>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _context.ProductCategories.AsQueryable();

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

        return new NovaERP.Application.Common.Models.PagedResult<ProductCategory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductCategory?> GetByIdAsync(Guid id)
    {
        return await _context.ProductCategories.FindAsync(id);
    }

    public Task AddAsync(ProductCategory category)
    {
        _context.ProductCategories.AddAsync(category);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ProductCategory category)
    {
        _context.ProductCategories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ProductCategory category)
    {
        _context.ProductCategories.Remove(category);
        return Task.CompletedTask;
    }
}
