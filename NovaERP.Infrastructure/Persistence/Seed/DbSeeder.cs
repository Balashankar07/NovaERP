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
    }
}