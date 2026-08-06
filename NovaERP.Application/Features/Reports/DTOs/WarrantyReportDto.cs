namespace NovaERP.Application.Features.Reports.DTOs;

public class WarrantyReportDto
{
    public Guid WarrantyId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
