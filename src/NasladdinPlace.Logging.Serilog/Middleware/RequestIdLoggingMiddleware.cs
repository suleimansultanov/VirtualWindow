using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NasladdinPlace.Logging.Serilog.Constants;
using Serilog.Context;

namespace NasladdinPlace.Logging.Serilog.Middleware
{
    public class RequestIdLoggingMiddleware
    {   
        private readonly RequestDelegate _next;

        public RequestIdLoggingMiddleware(RequestDelegate next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return InvokeAuxAsync(context);
        }

        private async Task InvokeAuxAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;

            using (LogContext.PushProperty(SerilogPropertyNames.RequestId, requestId))
            {
                await _next.Invoke(context);
            }
        }
    }
}