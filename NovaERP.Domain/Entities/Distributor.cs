using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class Distributor : AuditableEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation Property
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
