using NovaERP.Application.Common.Models;
﻿using NovaERP.Application.Features.Users.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto> CreateAsync(CreateUserDto dto);

    Task UpdateAsync(Guid id, UpdateUserDto dto);

    Task DeleteAsync(Guid id);
}