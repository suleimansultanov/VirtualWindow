using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Logging.Mvc.Middleware.RequestResponse
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ICollection<ILoggingPermission> _loggingPermissions;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next, 
            ILogger logger,
            IEnumerable<ILoggingPermission> loggingPermissions)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (loggingPermissions == null)
                throw new ArgumentNullException(nameof(loggingPermissions));
            
            _next = next;
            _logger = logger;
            _loggingPermissions = loggingPermissions.ToImmutableList();
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return InvokeAuxAsync(context);
        }

        private async Task InvokeAuxAsync(HttpContext context)
        {
            var request = await FormatRequestAsync(context.Request);

            var originalBodyStream = context.Response.Body;

            string response;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                catch (Exception)
                {
                    context.Response.Body = originalBodyStream;
                    throw;
                }

                response = await FormatResponseAsync(context.Response);

                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }

            _logger.LogInfo(message: $"Request: {request}");
            _logger.LogInfo(message: $"Response: {response}");
        }

        private async Task<string> FormatRequestAsync(HttpRequest request)
        {
            var loggingRequestBodyCheckResult = CheckWhetherLoggingRequestBodyAllowed(request);
            if (!loggingRequestBodyCheckResult.Succeeded)
            {
                var loggingQueryStringCheckResult = CheckWhetherLoggingQueryStringAllowed(request);
                return FormatRequestUri(request, loggingQueryStringCheckResult)
                    .Append($" Request body was not included because: {loggingRequestBodyCheckResult.Error}.")
                    .ToString();
            }

            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var bodyAsText = Encoding.UTF8.GetString(buffer);
            
            request.Body.Position = 0;

            var requestMessageBuilder = FormatRequestUri(request, CheckWhetherLoggingQueryStringAllowed(request));

            if (!string.IsNullOrWhiteSpace(bodyAsText))
                requestMessageBuilder.AppendLine(bodyAsText);
            
            return requestMessageBuilder.ToString();
        }

        private async Task<string> FormatResponseAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            
            var responseTextBuilder = new StringBuilder();
            
            responseTextBuilder.Append(response.StatusCode);
            
            var loggingResponseBodyCheckResult = CheckWhetherLoggingResponseBodyAllowed(response);
            if (!loggingResponseBodyCheckResult.Succeeded)
            {
                return responseTextBuilder
                    .Append(". Response body was not included because: ")
                    .Append(loggingResponseBodyCheckResult.Error)
                    .ToString();
            }

            var text = await new StreamReader(response.Body).ReadToEndAsync().ConfigureAwait(false);

            response.Body.Seek(0, SeekOrigin.Begin);

            if (!string.IsNullOrWhiteSpace(text))
            {
                responseTextBuilder
                    .Append(": ")
                    .Append(text);
            }

            return responseTextBuilder.ToString();
        }

        private static StringBuilder FormatRequestUri(HttpRequest request, Result loggingQueryStringCheckResult)
        {
            var requestUriBuilder = new StringBuilder();
            requestUriBuilder.Append($"{request.Scheme}://{request.Host}{request.Path}");

            if (loggingQueryStringCheckResult.Succeeded)
            {
                var queryString = request.QueryString.ToString();

                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    requestUriBuilder
                        .Append("?")
                        .Append(queryString);
                }
            }
            else
            {
                requestUriBuilder
                    .Append(" Request query string was not provided because: ")
                    .Append(loggingQueryStringCheckResult.Error);
            }

            return requestUriBuilder;
        }

        private Result CheckWhetherLoggingRequestBodyAllowed(HttpRequest request)
        {
            return CreateResultFromLoggingPermissionsMatchingPredicate(lp => lp.ShouldLogRequestBody(request));
        }

        private Result CheckWhetherLoggingResponseBodyAllowed(HttpResponse response)
        {
            return CreateResultFromLoggingPermissionsMatchingPredicate(lp => lp.ShouldLogResponseBody(response));
        }

        private Result CheckWhetherLoggingQueryStringAllowed(HttpRequest request)
        {
            return CreateResultFromLoggingPermissionsMatchingPredicate(lp => lp.ShouldLogRequestQueryString(request));
        }

        private Result CreateResultFromLoggingPermissionsMatchingPredicate(Predicate<ILoggingPermission> predicate)
        {
            var failedLoggingPermissionsDescriptions = _loggingPermissions
                .Where(lp => !predicate(lp))
                .Select(lp => lp.Description)
                .ToImmutableList();
            
            var failedLoggingPermissionDescriptionsAsString = 
                string.Join(" ", failedLoggingPermissionsDescriptions);

            return !string.IsNullOrWhiteSpace(failedLoggingPermissionDescriptionsAsString) 
                ? Result.Failure(failedLoggingPermissionDescriptionsAsString) 
                : Result.Success();
        }
    }
}