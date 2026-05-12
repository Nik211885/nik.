using backend.Exceptions;
using System.Text.Json;

namespace backend.Pipes.Middlewares;

/// <summary>
/// Translates <see cref="NotFoundException"/> and <see cref="BadRequestException"/> to
/// structured JSON error responses instead of letting them surface as 500s.
/// </summary>
public class ExeceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExeceptionHandlingMiddleware> _logger;

    /// <summary>Initialises the middleware with required dependencies.</summary>
    public ExeceptionHandlingMiddleware(ILogger<ExeceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "exception.internal");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(body);
    }
}
