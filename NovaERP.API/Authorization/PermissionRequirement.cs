using Microsoft.AspNetCore.Authorization;

namespace NovaERP.API.Authorization;

/// <summary>
/// Carries the name of the permission required to access a resource.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionRequirement(string permissionName)
    {
        PermissionName = permissionName;
    }
}
