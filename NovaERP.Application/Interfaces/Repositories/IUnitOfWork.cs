namespace NovaERP.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IRoleRepository Roles { get; }

    ICompanyRepository Companies { get; }

    IPermissionRepository Permissions { get; }

    IRolePermissionRepository RolePermissions { get; }

    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync();
}