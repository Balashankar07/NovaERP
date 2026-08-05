using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class SalesOrderItem : AuditableEntity
{
    public Guid SalesOrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation Properties
    public SalesOrder? SalesOrder { get; set; }
    public Product? Product { get; set; }
}
