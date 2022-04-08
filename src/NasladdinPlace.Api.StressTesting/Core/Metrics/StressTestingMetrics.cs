using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.StressTesting.Core.Metrics
{
    public class StressTestingMetrics : IReadOnlyStressTestingMetrics
    {
        public int ConcurrentRequestsNumber { get; set; }
        public TimeSpan AverageRequestLongevity { get; set; }
        public TimeSpan MinRequestLongevity { get; set; }
        public TimeSpan MaxRequestLongevity { get; set; }
        public ICollection<Exception> Exceptions { get; set; }
        public string StressTestName { get; set; }

        public StressTestingMetrics()
        {
            Exceptions = new Collection<Exception>();
        }
    }
}