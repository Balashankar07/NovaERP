using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByCodeAsync(string code);
}
