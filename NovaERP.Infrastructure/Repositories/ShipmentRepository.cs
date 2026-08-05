using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class ShipmentRepository : Repository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Shipment?> GetShipmentWithDetailsAsync(Guid id)
    {
        return await _context.Shipments
            .Include(s => s.ShipmentItems)
                .ThenInclude(si => si.Product)
            .Include(s => s.SalesOrder)
            .FirstOrDefaultAsync(s => s.Id == id);
    }


}
