using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NasladdinPlace.Logging.Serilog.Constants;
using Serilog.Context;

namespace NasladdinPlace.Logging.Serilog.Middleware
{
    public class UserIdLoggingMiddleware
    {   
        private readonly RequestDelegate _next;
        private readonly IdentityOptions _identityOptions;

        public UserIdLoggingMiddleware(RequestDelegate next, IOptions<IdentityOptions> identityOptionsAccessor)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (identityOptionsAccessor == null)
                throw new ArgumentNullException(nameof(identityOptionsAccessor));
            
            _next = next;
            _identityOptions = identityOptionsAccessor.Value;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return InvokeAuxAsync(context);
        }

        private async Task InvokeAuxAsync(HttpContext context)
        {
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirstValue(_identityOptions.ClaimsIdentity.UserIdClaimType);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    using (LogContext.PushProperty(SerilogPropertyNames.UserId, userId))
                    {
                        await _next.Invoke(context);
                    }
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}