using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.QualityInspections.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IQualityInspectionService
{
    Task<PagedResult<QualityInspectionDto>> GetQualityInspectionsAsync(
        int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<QualityInspectionDto> GetQualityInspectionByIdAsync(Guid id);
    
    Task<QualityInspectionDto> CreateAsync(CreateQualityInspectionDto dto, Guid? currentUserId);
    Task<QualityInspectionDto> UpdateAsync(Guid id, UpdateQualityInspectionDto dto, Guid? currentUserId);
    Task DeleteAsync(Guid id, Guid? currentUserId);
    
    Task<QualityInspectionDto> StartAsync(Guid id, Guid? currentUserId);
    Task<QualityInspectionDto> CompleteAsync(Guid id, Guid? currentUserId);
    Task<QualityInspectionDto> CancelAsync(Guid id, string reason, Guid? currentUserId);
    
    Task<QualityInspectionDto> AddDefectAsync(Guid inspectionId, CreateQualityDefectDto dto, Guid? currentUserId);
    Task RemoveDefectAsync(Guid inspectionId, Guid defectId, Guid? currentUserId);
}
