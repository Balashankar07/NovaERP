using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace NovaERP.API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicies(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("NovaERPCors", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:3000",
                        "http://localhost:5174")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicies(
        this IApplicationBuilder app)
    {
        app.UseCors("NovaERPCors");
        return app;
    }
}
