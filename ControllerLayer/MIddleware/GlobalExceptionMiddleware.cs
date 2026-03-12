// File: ControllerLayer/Middleware/GlobalExceptionMiddleware.cs
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ControllerLayer.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ControllerLayer.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled exception occurred.");

                if (httpContext.Response.HasStarted)
                {
                    throw;
                }

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                ErrorResponse response = new ErrorResponse
                {
                    TimestampUtc = DateTime.UtcNow,
                    Status = httpContext.Response.StatusCode,
                    Message = exception.Message,
                    Path = httpContext.Request.Path.ToString()
                };

                string json = JsonSerializer.Serialize(response);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}