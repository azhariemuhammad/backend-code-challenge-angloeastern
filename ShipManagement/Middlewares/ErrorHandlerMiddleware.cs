using System.Net;
using System.Text.Json;

namespace ShipManagement.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                (HttpStatusCode status, string message) = ex switch
                {
                    KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                    UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied."),
                    ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters."),
                    InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
                };

                var result = JsonSerializer.Serialize(new { error = message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)status;
                await context.Response.WriteAsync(result);
            }
        }

    }
}
