using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace The_Last_Dance_Project.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                Status = context.Response.StatusCode,
                Message = "Internal Server Error. Please contact support.",
                Detailed = exception.Message // Chỉ nên hiển thị trong môi trường Development
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
