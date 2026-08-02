namespace NovaERP.Application.Features.Users.DTOs;

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }

    public Guid RoleId { get; set; }

    public bool IsActive { get; set; }
}