using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(AppDbContext context)
        : base(context)
    {
    }
}
