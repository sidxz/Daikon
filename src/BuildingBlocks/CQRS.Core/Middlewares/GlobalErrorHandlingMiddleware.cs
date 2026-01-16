using System.Net;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public sealed class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

    public GlobalErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalErrorHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var baseEx = exception.GetBaseException();

        var statusCode = HttpStatusCode.InternalServerError;
        var response = new BaseResponse
        {
            Message = "An internal server error occurred"
        };

        switch (baseEx)
        {
            case ResourceNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                response.Message = baseEx.Message;
                _logger.LogWarning(exception, "Resource not found");
                break;

            case DuplicateEntityRequestException:
                statusCode = HttpStatusCode.Conflict;
                response.Message = baseEx.Message;
                _logger.LogWarning(exception, "Duplicate entity request");
                break;

            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                response.Message = baseEx.Message;
                _logger.LogWarning(exception, "Validation failed");
                break;

            case ArgumentNullException:
            case ArgumentException:
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                response.Message = baseEx.Message;
                _logger.LogWarning(exception, "Bad request");
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                response.Message = "Access is denied due to invalid credentials";
                _logger.LogWarning(exception, "Unauthorized access");
                break;

            case AuthenticationException:
                statusCode = HttpStatusCode.Unauthorized;
                response.Message = "Authentication failed";
                _logger.LogWarning(exception, "Authentication failed");
                break;

            case TimeoutException:
                statusCode = HttpStatusCode.GatewayTimeout;
                response.Message = "The request timed out. Please try again later.";
                _logger.LogWarning(exception, "Request timed out");
                break;

            case TaskCanceledException:
                statusCode = HttpStatusCode.RequestTimeout;
                response.Message = "The request was canceled or timed out.";
                _logger.LogWarning(exception, "Request canceled or timed out");
                break;

            case HttpRequestException:
                statusCode = HttpStatusCode.ServiceUnavailable;
                response.Message = "An error occurred while calling an external service. Please try again later.";
                _logger.LogWarning(exception, "External service error");
                break;

            case NotImplementedException:
                statusCode = HttpStatusCode.NotImplemented;
                response.Message = "This feature is not implemented yet.";
                _logger.LogWarning(exception, "Not implemented");
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "The response has already started, unable to write error response.");
            return Task.CompletedTask;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
