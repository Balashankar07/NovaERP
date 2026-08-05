namespace NovaERP.Application.Features.Shipments.DTOs;

public class ShipmentItemDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
}
