using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeatherDashboard.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var problem = new ProblemDetails { Instance = context.Request.Path };
        int status;

        switch (ex)
        {
            case InvalidOperationException ioe:
                status = StatusCodes.Status404NotFound;
                problem.Title = "Not Found";
                problem.Detail = ioe.Message;
                _logger.LogWarning(ioe, "Resource not found for {Path}", context.Request.Path);
                break;

            case OperationCanceledException oce:
                status = StatusCodes.Status504GatewayTimeout;
                problem.Title = "Request Timeout";
                problem.Detail = "The request was canceled or timed out.";
                _logger.LogWarning(oce, "Request canceled/timeout for {Path}", context.Request.Path);
                break;

            default:
                status = StatusCodes.Status500InternalServerError;
                problem.Title = "An unexpected error occurred";
                problem.Detail = ex.Message;
                _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
                break;
        }

        problem.Status = status;
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}