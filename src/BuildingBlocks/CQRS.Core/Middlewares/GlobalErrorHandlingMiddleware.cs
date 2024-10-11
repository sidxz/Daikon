using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;

public class GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Proceed to the next middleware/controller
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var response = new BaseResponse { Message = "An internal server error occurred" };

        // Map specific exceptions to appropriate HTTP status codes and messages
        switch (exception)
        {
            case ResourceNotFoundException _:
                code = HttpStatusCode.NotFound;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Resource not found");
                break;

            case InvalidOperationException _:
                code = HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Invalid operation");
                break;

            case ArgumentNullException _:
                code = HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Argument null");
                break;

            case DuplicateEntityRequestException _:
                code = HttpStatusCode.Conflict;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Duplicate entity request");
                break;

            case UnauthorizedAccessException _:
                code = HttpStatusCode.Unauthorized;
                response.Message = "Access is denied due to invalid credentials";
                _logger.LogWarning(exception, "Unauthorized access");
                break;

            case AuthenticationException _:
                code = HttpStatusCode.Unauthorized;
                response.Message = "Authentication failed";
                _logger.LogWarning(exception, "Authentication failed");
                break;

            case ValidationException _:
                code = HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Validation failed");
                break;

            case TimeoutException _:
                code = HttpStatusCode.GatewayTimeout;
                response.Message = "The request timed out. Please try again later.";
                _logger.LogWarning(exception, "Request timed out");
                break;

            case HttpRequestException _:
                code = HttpStatusCode.ServiceUnavailable;
                response.Message = "An error occurred while calling an external service. Please try again later.";
                _logger.LogWarning(exception, "External service error");
                break;

            case NotImplementedException _:
                code = HttpStatusCode.NotImplemented;
                response.Message = "This feature is not implemented yet.";
                _logger.LogWarning(exception, "Not implemented");
                break;

            case TaskCanceledException _:
                code = HttpStatusCode.RequestTimeout;
                response.Message = "The request was canceled or timed out.";
                _logger.LogWarning(exception, "Request canceled or timed out");
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var jsonResponse = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(jsonResponse);
    }
}
