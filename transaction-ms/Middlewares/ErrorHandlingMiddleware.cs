using System.Net;
using System.Text.Json;
using transaction_application.Common.Exceptions;

namespace transaction_ms.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                _logger.LogWarning(vex, "Validation failed: {TraceId}", context.TraceIdentifier);
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, new
                {
                    type = $"https://httpstatuses.io/{HttpStatusCode.BadRequest}",
                    title = "Validation failed",
                    status = (int)HttpStatusCode.BadRequest,
                    errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }),
                    traceId = context.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {TraceId}", context.TraceIdentifier);
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
                {
                    type = $"https://httpstatuses.io/{HttpStatusCode.InternalServerError}",
                    title = "Internal Server Error",
                    status = (int)HttpStatusCode.InternalServerError,
                    detail = ex.Message,
                    traceId = context.TraceIdentifier
                });
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object problem)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
