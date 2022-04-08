using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission
{
    public class ForbidResponseBodyLogging : ILoggingPermission
    {
        public bool ShouldLogRequestBody(HttpRequest request)
        {
            return true;
        }

        public bool ShouldLogResponseBody(HttpResponse response)
        {
            return false;
        }

        public bool ShouldLogRequestQueryString(HttpRequest request)
        {
            return true;
        }

        public string Description => "Logging of response body is not allowed.";
    }
}