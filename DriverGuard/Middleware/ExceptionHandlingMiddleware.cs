using DriverGuard.Backend.Exceptions;
using System.Net;
using System.Text.Json;

namespace DriverGuard.Backend.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business exception");

                await WriteErrorAsync(
                    context,
                    ex.StatusCode,
                    ex.Message
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument exception");

                await WriteErrorAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteErrorAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Внутрішня помилка сервера"
                );
            }
        }

        private static async Task WriteErrorAsync(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = message,
                status = statusCode,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}
