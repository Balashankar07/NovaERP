using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId =>
        GetGuidClaim(ClaimTypes.NameIdentifier);

    public string Email =>
        GetStringClaim(ClaimTypes.Email);

    public string Role =>
        GetStringClaim(ClaimTypes.Role);

    public Guid CompanyId =>
        GetGuidClaim("CompanyId");

    public Guid BranchId =>
        GetGuidClaim("BranchId");

    private Guid GetGuidClaim(string claimType)
    {
        var value = User?.FindFirst(claimType)?.Value;

        return Guid.TryParse(value, out var guid)
            ? guid
            : Guid.Empty;
    }

    private string GetStringClaim(string claimType)
    {
        return User?.FindFirst(claimType)?.Value ?? string.Empty;
    }
}