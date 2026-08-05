using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class WarrantyRepository : Repository<Warranty>, IWarrantyRepository
{
    public WarrantyRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber)
    {
        return await _context.Set<Warranty>().AnyAsync(w => w.SerialNumber == serialNumber);
    }

    public async Task<bool> ExistsForProductAndShipmentAsync(Guid productId, Guid shipmentId)
    {
        return await _context.Set<Warranty>().AnyAsync(w => w.ProductId == productId && w.ShipmentId == shipmentId);
    }
}
