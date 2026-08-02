using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context)
        : base(context)
    {
    }
}
