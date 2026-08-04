using Microsoft.EntityFrameworkCore;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<User> Users => Set<User>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}