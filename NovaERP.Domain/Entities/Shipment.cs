using NovaERP.Domain.Common;
using NovaERP.Domain.Enums;

namespace NovaERP.Domain.Entities;

public class Shipment : AuditableEntity
{
    public Guid SalesOrderId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string CourierName { get; set; } = string.Empty;
    public DateTime? DispatchDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

    // Navigation Properties
    public SalesOrder? SalesOrder { get; set; }
    public ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
}
