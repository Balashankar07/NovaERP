using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class BOMItemRepository : IBOMItemRepository
{
    private readonly AppDbContext _context;

    public BOMItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(BOMItem bomItem)
    {
        _context.BOMItems.AddAsync(bomItem);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(BOMItem bomItem)
    {
        _context.BOMItems.Update(bomItem);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(BOMItem bomItem)
    {
        _context.BOMItems.Remove(bomItem);
        return Task.CompletedTask;
    }
}
