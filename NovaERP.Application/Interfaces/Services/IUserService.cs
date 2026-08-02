using NovaERP.Application.Features.Users.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto> CreateAsync(CreateUserDto dto);

    Task UpdateAsync(Guid id, UpdateUserDto dto);

    Task DeleteAsync(Guid id);
}