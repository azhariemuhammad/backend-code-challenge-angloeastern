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
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (status, message) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied."),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}
