using NovaERP.API.Middleware;

namespace NovaERP.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}