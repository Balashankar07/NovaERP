using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities
{
    public class Permission : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string Module { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
