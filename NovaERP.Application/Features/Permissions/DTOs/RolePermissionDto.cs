namespace NovaERP.Application.Features.Permissions.DTOs
{
    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
}
