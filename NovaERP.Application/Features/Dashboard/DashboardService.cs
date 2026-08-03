using NovaERP.Application.Features.Dashboard;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.Application.Features.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardDto> GetSummaryAsync()
    {
        var users     = await _unitOfWork.Users.GetAllAsync();
        var roles     = await _unitOfWork.Roles.GetAllAsync();
        var companies = await _unitOfWork.Companies.GetAllAsync();

        return new DashboardDto
        {
            TotalUsers     = users.TotalCount,
            TotalRoles     = roles.TotalCount,
            TotalCompanies = companies.TotalCount
        };
    }
}
