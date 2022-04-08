using Microsoft.AspNetCore.Http;
using System;

namespace NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission
{
    public class ForbidTooLongContentLogging : ILoggingPermission
    {
        private const long KbInBytes = 1024;

        private const long DefaultContentLengthInBytes = KbInBytes;

        private readonly long _maxContentLengthInBytes;

        public ForbidTooLongContentLogging()
            : this(DefaultContentLengthInBytes)
        {
            // intentionally left empty
        }

        public ForbidTooLongContentLogging(long maxContentLengthInBytes)
        {
            if (maxContentLengthInBytes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxContentLengthInBytes),
                    maxContentLengthInBytes,
                    $"{nameof(maxContentLengthInBytes)} should be greater than zero. But found: {maxContentLengthInBytes}"
                );

            _maxContentLengthInBytes = maxContentLengthInBytes;
        }

        public bool ShouldLogRequestBody(HttpRequest request)
        {
            return !request.ContentLength.HasValue || request.ContentLength < _maxContentLengthInBytes;
        }

        public bool ShouldLogResponseBody(HttpResponse response)
        {
            return !response.ContentLength.HasValue || response.ContentLength < _maxContentLengthInBytes;
        }

        public bool ShouldLogRequestQueryString(HttpRequest request)
        {
            return true;
        }

        public string Description => "Content is too long.";
    }
}