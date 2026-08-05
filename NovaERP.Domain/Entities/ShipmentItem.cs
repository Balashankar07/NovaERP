using System.ComponentModel.DataAnnotations.Schema;
using NovaERP.Domain.Common;

namespace NovaERP.Domain.Entities;

public class ShipmentItem : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveredQuantity { get; set; }

    // Navigation Properties
    public Shipment? Shipment { get; set; }
    public Product? Product { get; set; }
}
