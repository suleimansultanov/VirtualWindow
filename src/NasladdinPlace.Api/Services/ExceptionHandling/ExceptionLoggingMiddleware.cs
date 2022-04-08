using Microsoft.AspNetCore.Http;
using NasladdinPlace.Api.Services.ExceptionHandling.Models;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.ExceptionHandling
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.Error($"Global exception: {ex}.");
                await ReturnInternalServerErrorResponse(context);
            }
        }

        private static Task ReturnInternalServerErrorResponse(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorDetails = new ErrorDetails
            {
                Message = "An unhandled error occured during execution of the request.",
                StatusCode = context.Response.StatusCode
            };
            return context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}