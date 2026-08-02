namespace NovaERP.Application.Features.Users.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }

    public Guid RoleId { get; set; }

    public bool IsActive { get; set; }
}