using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

using NovaERP.Application.Authentication.Commands.Login;
using NovaERP.Application.Authentication.Validators;
using NovaERP.Application.Behaviors;

namespace NovaERP.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<LoginCommand>();

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

        return services;
    }
}