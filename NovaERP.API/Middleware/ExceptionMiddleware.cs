using System.Net;
using System.Text.Json;

using FluentValidation;
using NovaERP.Application.Common.Exceptions;

namespace NovaERP.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                response.StatusCode = 400;
                response.Success = false;
                response.Message = "Validation Failed";
                response.Errors = validationException.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                break;

            case UnauthorizedAccessException:

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                response.StatusCode = 401;
                response.Success = false;
                response.Message = "Unauthorized";

                break;

            case BadRequestException badRequestEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = 400;
                response.Success = false;
                response.Message = badRequestEx.Message;
                break;

            case ConflictException conflictEx:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response.StatusCode = 409;
                response.Success = false;
                response.Message = conflictEx.Message;
                break;

            case KeyNotFoundException:

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Resource Not Found";

                break;

            default:

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Internal Server Error";

                break;
        }

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string>? Errors { get; set; }
}