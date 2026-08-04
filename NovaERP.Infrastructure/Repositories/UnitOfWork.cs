using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Infrastructure.Persistence.Context;
using NovaERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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
    public IBOMRepository BOMs { get; private set; }
    public IBOMItemRepository BOMItems { get; private set; }
    public ISupplierRepository Suppliers { get; private set; }
    public IPurchaseOrderRepository PurchaseOrders { get; private set; }
    public IGoodsReceiptRepository GoodsReceipts { get; private set; }
    public IWarehouseRepository Warehouses { get; private set; }
    public IWarehouseLocationRepository WarehouseLocations { get; private set; }

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
        IProductRepository productRepository,
        IBOMRepository bomRepository,
        IBOMItemRepository bomItemRepository,
        ISupplierRepository supplierRepository)
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
        BOMs = new BOMRepository(_context);
        BOMItems = new BOMItemRepository(_context);
        Suppliers = new SupplierRepository(_context);
        PurchaseOrders = new PurchaseOrderRepository(_context);
        GoodsReceipts = new GoodsReceiptRepository(_context);
        Warehouses = new WarehouseRepository(_context);
        WarehouseLocations = new WarehouseLocationRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
    }

    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CurrentTransaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CurrentTransaction.RollbackAsync();
        }
    }
}