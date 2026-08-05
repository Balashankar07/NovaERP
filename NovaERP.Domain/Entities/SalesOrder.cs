using NovaERP.Domain.Common;
using NovaERP.Domain.Enums;

namespace NovaERP.Domain.Entities;

public class SalesOrder : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid DistributorId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    
    // Navigation Properties
    public Distributor? Distributor { get; set; }
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
}
