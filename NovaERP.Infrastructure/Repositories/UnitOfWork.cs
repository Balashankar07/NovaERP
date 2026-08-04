using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Infrastructure.Persistence.Context;
using NovaERP.Infrastructure.Repositories;

namespace NovaERP.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public ICompanyRepository Companies { get; private set; }
    public IPermissionRepository Permissions { get; private set; }
    public IRolePermissionRepository RolePermissions { get; private set; }
    public IAuditLogRepository AuditLogs { get; private set; }

    public IProductCategoryRepository ProductCategories { get; private set; }
    public IBrandRepository Brands { get; private set; }
    public IUnitRepository Units { get; private set; }
    public IProductRepository Products { get; private set; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICompanyRepository companyRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IAuditLogRepository auditLogRepository,
        IProductCategoryRepository productCategoryRepository,
        IBrandRepository brandRepository,
        IUnitRepository unitRepository,
        IProductRepository productRepository)
    {
        _context = context;
        Users = userRepository;
        Roles = roleRepository;
        Companies = companyRepository;
        Permissions = permissionRepository;
        RolePermissions = rolePermissionRepository;
        AuditLogs = auditLogRepository;

        ProductCategories = productCategoryRepository;
        Brands = brandRepository;
        Units = unitRepository;
        Products = productRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}