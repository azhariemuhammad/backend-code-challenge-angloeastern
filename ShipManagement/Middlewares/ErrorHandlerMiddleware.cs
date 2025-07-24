
using System.Text.Json;

namespace ShipManagement.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                (HttpStatusCode status, string message) = ex switch
                {
                    KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                    UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied."),
                    ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters."),
                    InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                    DuplicateShipCodeException => (HttpStatusCode.Conflict, "Ship code already exists."),
                    DuplicateUserNameException => (HttpStatusCode.Conflict, "User name already exists."),
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
