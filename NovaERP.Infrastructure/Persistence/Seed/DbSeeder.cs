using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // ==========================
        // Seed Company
        // ==========================
        if (!await context.Companies.AnyAsync())
        {
            var company = new Company
            {
                Name = "Nova Electronics",
                Code = "NOVA",
                Email = "info@novaerp.com",
                Phone = "+91 9999999999",
                Website = "https://novaerp.com",
                Address = "Head Office",
                City = "Kottayam",
                State = "Kerala",
                Country = "India",
                PostalCode = "686001",
                IsActive = true
            };

            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Roles
        // ==========================
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Name = "Super Admin" },
                new() { Name = "CEO" },
                new() { Name = "HR Manager" },
                new() { Name = "Finance Manager" },
                new() { Name = "Inventory Manager" },
                new() { Name = "Sales Manager" },
                new() { Name = "Employee" }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Permissions
        // ==========================
        var permissions = new List<Permission>
        {
            new() { Name = "Permissions.Users.View", Description = "View Users", Module = "Users" },
            new() { Name = "Permissions.Users.Create", Description = "Create Users", Module = "Users" },
            new() { Name = "Permissions.Users.Edit", Description = "Edit Users", Module = "Users" },
            new() { Name = "Permissions.Users.Delete", Description = "Delete Users", Module = "Users" },
            new() { Name = "Permissions.Roles.View", Description = "View Roles", Module = "Roles" },
            new() { Name = "Permissions.Roles.Create", Description = "Create Roles", Module = "Roles" },
            new() { Name = "Permissions.Roles.Edit", Description = "Edit Roles", Module = "Roles" },
            new() { Name = "Permissions.Roles.Delete", Description = "Delete Roles", Module = "Roles" },
            new() { Name = "Permissions.Dashboard.View", Description = "View Dashboard", Module = "Dashboard" },
            
            new() { Name = "Permissions.Products.View", Description = "View Products", Module = "Products" },
            new() { Name = "Permissions.Products.Create", Description = "Create Products", Module = "Products" },
            new() { Name = "Permissions.Products.Update", Description = "Edit Products", Module = "Products" },
            new() { Name = "Permissions.Products.Delete", Description = "Delete Products", Module = "Products" },

            new() { Name = "Permissions.ProductCategories.View", Description = "View Product Categories", Module = "Products" },
            new() { Name = "Permissions.ProductCategories.Create", Description = "Create Product Categories", Module = "Products" },
            new() { Name = "Permissions.ProductCategories.Update", Description = "Edit Product Categories", Module = "Products" },
            new() { Name = "Permissions.ProductCategories.Delete", Description = "Delete Product Categories", Module = "Products" },

            new() { Name = "Permissions.Brands.View", Description = "View Brands", Module = "Products" },
            new() { Name = "Permissions.Brands.Create", Description = "Create Brands", Module = "Products" },
            new() { Name = "Permissions.Brands.Update", Description = "Edit Brands", Module = "Products" },
            new() { Name = "Permissions.Brands.Delete", Description = "Delete Brands", Module = "Products" },

            new() { Name = "Permissions.Units.View", Description = "View Units", Module = "Products" },
            new() { Name = "Permissions.Units.Create", Description = "Create Units", Module = "Products" },
            new() { Name = "Permissions.Units.Update", Description = "Edit Units", Module = "Products" },
            new() { Name = "Permissions.Units.Delete", Description = "Delete Units", Module = "Products" },

            new() { Name = "Permissions.BOMs.View", Description = "View BOMs", Module = "BOMs" },
            new() { Name = "Permissions.BOMs.Create", Description = "Create BOMs", Module = "BOMs" },
            new() { Name = "Permissions.BOMs.Update", Description = "Edit BOMs", Module = "BOMs" },
            new() { Name = "Permissions.BOMs.Delete", Description = "Delete BOMs", Module = "BOMs" },

            new() { Name = "Permissions.Suppliers.View", Description = "View Suppliers", Module = "Suppliers" },
            new() { Name = "Permissions.Suppliers.Create", Description = "Create Suppliers", Module = "Suppliers" },
            new() { Name = "Permissions.Suppliers.Update", Description = "Edit Suppliers", Module = "Suppliers" },
            new() { Name = "Permissions.Suppliers.Delete", Description = "Delete Suppliers", Module = "Suppliers" },

            new() { Name = "Permissions.PurchaseOrders.View", Description = "View Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Create", Description = "Create Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Update", Description = "Edit Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Delete", Description = "Delete Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Submit", Description = "Submit Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Approve", Description = "Approve Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.PurchaseOrders.Reject", Description = "Reject Purchase Orders", Module = "PurchaseOrders" },
            new() { Name = "Permissions.GoodsReceipts.View", Description = "View Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Create", Description = "Create Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Update", Description = "Edit Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Delete", Description = "Delete Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Receive", Description = "Receive Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Complete", Description = "Complete Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.GoodsReceipts.Cancel", Description = "Cancel Goods Receipts", Module = "GoodsReceipts" },
            new() { Name = "Permissions.Warehouses.View", Description = "View Warehouses", Module = "WarehouseManagement" },
            new() { Name = "Permissions.Warehouses.Create", Description = "Create Warehouses", Module = "WarehouseManagement" },
            new() { Name = "Permissions.Warehouses.Update", Description = "Edit Warehouses", Module = "WarehouseManagement" },
            new() { Name = "Permissions.Warehouses.Delete", Description = "Delete Warehouses", Module = "WarehouseManagement" },
            new() { Name = "Permissions.WarehouseLocations.View", Description = "View Warehouse Locations", Module = "WarehouseManagement" },
            new() { Name = "Permissions.WarehouseLocations.Create", Description = "Create Warehouse Locations", Module = "WarehouseManagement" },
            new() { Name = "Permissions.WarehouseLocations.Update", Description = "Edit Warehouse Locations", Module = "WarehouseManagement" },
            new() { Name = "Permissions.WarehouseLocations.Delete", Description = "Delete Warehouse Locations", Module = "WarehouseManagement" }
        };

        foreach (var p in permissions)
        {
            if (!await context.Permissions.AnyAsync(x => x.Name == p.Name))
            {
                await context.Permissions.AddAsync(p);
            }
        }
        await context.SaveChangesAsync();

        // ==========================
        // Seed Admin User
        // ==========================
        if (!await context.Users.AnyAsync())
        {
            var company = await context.Companies.FirstAsync();

            var superAdminRole = await context.Roles
                .FirstAsync(r => r.Name == "Super Admin");

            var admin = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@novaerp.com",
                Phone = "+91 9999999999",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CompanyId = company.Id,
                RoleId = superAdminRole.Id,
                IsActive = true
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Employee User (for negative RBAC testing)
        // ==========================
        if (!await context.Users.AnyAsync(u => u.Email == "employee@novaerp.com"))
        {
            var company = await context.Companies.FirstAsync();
            var employeeRole = await context.Roles.FirstAsync(r => r.Name == "Employee");

            var emp = new User
            {
                FirstName = "Test",
                LastName = "Employee",
                Email = "employee@novaerp.com",
                Phone = "+91 8888888888",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                CompanyId = company.Id,
                RoleId = employeeRole.Id,
                IsActive = true
            };

            await context.Users.AddAsync(emp);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Super Admin Role Permissions
        // ==========================
        var superAdmin = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Super Admin");
        if (superAdmin != null)
        {
            var allPermissions = await context.Permissions.ToListAsync();
            var existingRolePermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == superAdmin.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var newRolePermissions = allPermissions
                .Where(p => !existingRolePermissions.Contains(p.Id))
                .Select(p => new RolePermission
                {
                    RoleId = superAdmin.Id,
                    PermissionId = p.Id
                }).ToList();

            if (newRolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(newRolePermissions);
                await context.SaveChangesAsync();
            }
        }

        // ==========================
        // Seed Product Categories
        // ==========================
        if (!await context.ProductCategories.AnyAsync())
        {
            var categories = new List<ProductCategory>
            {
                new() { Name = "Electronics", Description = "Electronic Devices and Accessories" },
                new() { Name = "Computers", Description = "Laptops, Desktops and Peripherals" },
                new() { Name = "Home Appliances", Description = "Appliances for Home" }
            };

            await context.ProductCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Brands
        // ==========================
        if (!await context.Brands.AnyAsync())
        {
            var brands = new List<Brand>
            {
                new() { Name = "Apple", Description = "Apple Inc." },
                new() { Name = "Samsung", Description = "Samsung Electronics" },
                new() { Name = "Sony", Description = "Sony Corporation" },
                new() { Name = "Dell", Description = "Dell Technologies" }
            };

            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }

        // ==========================
        // Seed Units
        // ==========================
        if (!await context.Units.AnyAsync())
        {
            var units = new List<Unit>
            {
                new() { Name = "Piece", Abbreviation = "pcs" },
                new() { Name = "Box", Abbreviation = "box" },
                new() { Name = "Kilogram", Abbreviation = "kg" }
            };

            await context.Units.AddRangeAsync(units);
            await context.SaveChangesAsync();
        }
    }
}