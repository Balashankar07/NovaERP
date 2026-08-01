using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;

using NovaERP.Infrastructure.Identity.JWT;
using NovaERP.Infrastructure.Identity.Security;
using NovaERP.Infrastructure.Persistence.Context;
using NovaERP.Infrastructure.Persistence.Repositories;
using NovaERP.Infrastructure.Repositories;
using NovaERP.Infrastructure.Services;

namespace NovaERP.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Repository Registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyService, CompanyService>();

        // JWT
        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICompanyService, CompanyService>();

        // Password Hashing
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}