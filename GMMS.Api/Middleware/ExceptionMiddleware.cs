using Azure;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace GMMS.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
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
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while Processing {Method} {Path}", context.Request.Method, context.Request.Path);

                await WriteResponse(
                    context,
                    HttpStatusCode.InternalServerError,
                    "A database error occurred.");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Request timeoutwhile Processing {Method} {Path}", context.Request.Method, context.Request.Path);

                await WriteResponse(
                    context,
                    HttpStatusCode.RequestTimeout,
                    "The request timed out.");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while Processing {Method} {Path}", context.Request.Method, context.Request.Path);

                await WriteResponse(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                IsSuccess = false,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}