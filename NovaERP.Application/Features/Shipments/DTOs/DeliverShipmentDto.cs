using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Shipments.DTOs;

public class DeliverShipmentDto
{
    [Required]
    public List<DeliverShipmentItemDto> DeliveredItems { get; set; } = new();
}

public class DeliverShipmentItemDto
{
    [Required]
    public Guid ShipmentItemId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Delivered quantity cannot be negative.")]
    public decimal DeliveredQuantity { get; set; }
}
