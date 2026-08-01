using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }

    public IRoleRepository Roles { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Users = new UserRepository(context);

        Roles = new RoleRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}