using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class WarrantyClaimRepository : Repository<WarrantyClaim>, IWarrantyClaimRepository
{
    public WarrantyClaimRepository(AppDbContext context) : base(context)
    {
    }
}
