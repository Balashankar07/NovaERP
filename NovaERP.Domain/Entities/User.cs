using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class User : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }

    public Guid RoleId { get; set; }

    public bool IsActive { get; set; } = true;

   

    // Navigation Property
    public Company Company { get; set; } = null!;
    public Role Role { get; set; } = null!;
}