using Microsoft.AspNetCore.Builder;

namespace NasladdinPlace.Api.Services.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static void UseExceptionHandlerWithLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionLoggingMiddleware>();
        }
    }
}