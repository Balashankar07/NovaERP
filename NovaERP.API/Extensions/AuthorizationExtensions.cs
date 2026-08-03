using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NovaERP.API.Authorization;

namespace NovaERP.API.Extensions;

/// <summary>
/// Registers the permission-based authorization infrastructure.
/// </summary>
public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(
        this IServiceCollection services)
    {
        // Register the handler that evaluates PermissionRequirement
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Register a dynamic policy provider that creates a PermissionRequirement-based
        // policy for any policy name that looks like a permission (e.g. "Users.Create").
        // Falls back to the default provider for standard policies (e.g. plain [Authorize]).
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}

/// <summary>
/// Dynamically creates an <see cref="AuthorizationPolicy"/> from a permission name at runtime.
/// Falls through to the default provider for any policy not matching a permission name.
/// </summary>
internal sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallback.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // If a conventional policy was registered explicitly, use it.
        var existingPolicy = await _fallback.GetPolicyAsync(policyName);
        if (existingPolicy is not null)
            return existingPolicy;

        // Otherwise treat the policy name as a permission name and build a policy on the fly.
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        return policy;
    }
}
