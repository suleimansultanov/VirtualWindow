using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.Api.StressTesting.Args.Constrants
{
    public static class CommandLineArguments
    {
        public const string UserName = "userName";
        public const string UserSecret = "secret";
        public const string BaseApiUrl = "baseApiUrl";
        public const string ConcurrentRequestsNumber = "concurrentRequestsNumber";

        public static readonly IReadOnlyCollection<string> All = new[]
            {
                UserName,
                UserSecret,
                BaseApiUrl,
                ConcurrentRequestsNumber
            }
            .ToImmutableList();
    }
}