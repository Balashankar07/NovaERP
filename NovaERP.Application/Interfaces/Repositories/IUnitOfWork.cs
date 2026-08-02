namespace NovaERP.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IRoleRepository Roles { get; }

    ICompanyRepository Companies { get; }

    Task<int> SaveChangesAsync();
}