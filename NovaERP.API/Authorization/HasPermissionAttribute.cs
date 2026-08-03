using Microsoft.AspNetCore.Authorization;

namespace NovaERP.API.Authorization;

/// <summary>
/// Applies a named permission policy to a controller or action.
/// The policy name equals the permission name (e.g. "Users.Create").
/// </summary>
/// <example>
/// [HasPermission("Users.Create")]
/// public async Task&lt;IActionResult&gt; Create(...) { ... }
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
