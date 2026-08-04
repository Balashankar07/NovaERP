namespace NovaERP.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IRoleRepository Roles { get; }

    ICompanyRepository Companies { get; }

    IPermissionRepository Permissions { get; }

    IRolePermissionRepository RolePermissions { get; }

    IAuditLogRepository AuditLogs { get; }

    IProductCategoryRepository ProductCategories { get; }

    IBrandRepository Brands { get; }

    IUnitRepository Units { get; }

    IProductRepository Products { get; }

    Task<int> SaveChangesAsync();
}