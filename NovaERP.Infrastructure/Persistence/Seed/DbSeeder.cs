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

        if (!await context.Users.AnyAsync())
        {
            var superAdminRole = await context.Roles
                .FirstAsync(r => r.Name == "Super Admin");

            var admin = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@novaerp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = superAdminRole.Id,
                IsActive = true
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}