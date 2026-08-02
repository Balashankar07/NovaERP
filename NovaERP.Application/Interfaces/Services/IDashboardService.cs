namespace NovaERP.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<NovaERP.Application.Features.Dashboard.DashboardDto> GetSummaryAsync();
}
