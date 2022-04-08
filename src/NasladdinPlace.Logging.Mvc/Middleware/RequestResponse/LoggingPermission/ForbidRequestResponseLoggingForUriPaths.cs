using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission
{
    public class ForbidRequestResponseLoggingForUriPaths : ILoggingPermission
    {
        private readonly IEnumerable<string> _paths;

        public ForbidRequestResponseLoggingForUriPaths(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            
            _paths = paths.Select(p => p.ToLower()).ToImmutableHashSet();
        }
        
        public bool ShouldLogRequestBody(HttpRequest request)
        {
            return CheckWhetherRequestPathAllowed(request);
        }

        public bool ShouldLogResponseBody(HttpResponse response)
        {
            return CheckWhetherRequestPathAllowed(response.HttpContext.Request);
        }

        public bool ShouldLogRequestQueryString(HttpRequest request)
        {
            return CheckWhetherRequestPathAllowed(request);
        }

        public string Description => "Content logging is forbidden for path.";

        private bool CheckWhetherRequestPathAllowed(HttpRequest request)
        {
            var requestPath = request.Path.ToString().ToLower();
            return !_paths.Any(p => requestPath.Contains(p));
        }
    }
}