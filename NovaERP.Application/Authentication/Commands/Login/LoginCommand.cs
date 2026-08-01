using MediatR;
using NovaERP.Application.Authentication.DTOs;

namespace NovaERP.Application.Authentication.Commands.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}