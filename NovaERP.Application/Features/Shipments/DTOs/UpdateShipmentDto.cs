using System.ComponentModel.DataAnnotations;

namespace NovaERP.Application.Features.Shipments.DTOs;

public class UpdateShipmentDto
{
    [MaxLength(100)]
    public string? TrackingNumber { get; set; }

    [MaxLength(100)]
    public string? CourierName { get; set; }
}
