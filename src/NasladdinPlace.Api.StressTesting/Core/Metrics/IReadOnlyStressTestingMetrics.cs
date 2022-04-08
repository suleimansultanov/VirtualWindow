using System;
using System.Collections.Generic;

namespace NasladdinPlace.Api.StressTesting.Core.Metrics
{
    public interface IReadOnlyStressTestingMetrics
    {
        int ConcurrentRequestsNumber { get; }
        TimeSpan AverageRequestLongevity { get; }
        TimeSpan MinRequestLongevity { get; }
        TimeSpan MaxRequestLongevity { get; }
        ICollection<Exception> Exceptions { get; }
        string StressTestName { get; }
    }
}