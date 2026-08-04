using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _context.Products
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Unit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || x.ProductCode.Contains(search) || x.SKU.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            query = sortBy.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "code" => isDesc ? query.OrderByDescending(x => x.ProductCode) : query.OrderBy(x => x.ProductCode),
                "sku" => isDesc ? query.OrderByDescending(x => x.SKU) : query.OrderBy(x => x.SKU),
                "createdat" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
            };
        }

        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new NovaERP.Application.Common.Models.PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product?> GetByCodeAsync(string code)
    {
        return await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.ProductCode == code);
    }

    public Task AddAsync(Product product)
    {
        _context.Products.AddAsync(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }
}
