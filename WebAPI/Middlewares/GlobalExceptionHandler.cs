using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;

namespace WebAPI.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly bool _isDebugMode;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _isDebugMode = environment.IsDevelopment();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled exception: {ex.Message}");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = _isDebugMode ? ex.ToString() : "An unhandled exception occurred.",
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                };

                context.Response.StatusCode = problemDetails.Status.Value;
                var json = JsonConvert.SerializeObject(problemDetails);
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
        }
    }

}
