using NovaERP.Domain.Enums;

namespace NovaERP.Application.Features.Warranties.DTOs;

public class UpdateWarrantyDto
{
    public WarrantyStatus? Status { get; set; }
}
