using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NovaERP.Infrastructure.Identity.JWT;

namespace NovaERP.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");

        services.Configure<JwtSettings>(jwtSection);

        var jwtSettings = jwtSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");

        Console.WriteLine("================================");
        Console.WriteLine("JWT VALIDATION");
        Console.WriteLine($"Issuer   : {jwtSettings.Issuer}");
        Console.WriteLine($"Audience : {jwtSettings.Audience}");
        Console.WriteLine($"Secret   : {jwtSettings.SecretKey}");
        Console.WriteLine($"Expiry   : {jwtSettings.ExpiryMinutes}");
        Console.WriteLine("================================");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;

            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

        services.AddAuthorization();

        return services;
    }
}