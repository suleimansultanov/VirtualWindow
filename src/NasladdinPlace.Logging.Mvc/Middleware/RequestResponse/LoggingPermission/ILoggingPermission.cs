using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission
{
    public interface ILoggingPermission
    {
        bool ShouldLogRequestBody(HttpRequest request);
        bool ShouldLogResponseBody(HttpResponse response);
        bool ShouldLogRequestQueryString(HttpRequest request);
        string Description { get; }
    }
}