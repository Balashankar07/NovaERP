using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class Unit : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Abbreviation { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
