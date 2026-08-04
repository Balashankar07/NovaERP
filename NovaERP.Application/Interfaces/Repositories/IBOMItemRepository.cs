using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IBOMItemRepository
{
    Task AddAsync(BOMItem bomItem);
    Task UpdateAsync(BOMItem bomItem);
    Task DeleteAsync(BOMItem bomItem);
}
