using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.StressTesting.Core.Metrics;

namespace NasladdinPlace.Api.StressTesting.Models
{
    public class StressTestingReport
    {
        public ICollection<IReadOnlyStressTestingMetrics> StressTestingUseCasesMetrics { get; }

        public StressTestingReport(ICollection<IReadOnlyStressTestingMetrics> stressTestingUseCasesMetrics)
        {
            if (stressTestingUseCasesMetrics == null)
                throw new ArgumentNullException(nameof(stressTestingUseCasesMetrics));

            StressTestingUseCasesMetrics = stressTestingUseCasesMetrics;
        }

        public override string ToString()
        {
            return string.Join("\n", StressTestingUseCasesMetrics.Select(m =>
                $"StressTestName={m.StressTestName}\n" +
                $"Concurrent requests number={m.ConcurrentRequestsNumber}\n" +
                $"Avg request longevity={m.AverageRequestLongevity.TotalSeconds} sec\n" +
                $"Min request longevity={m.MinRequestLongevity.TotalSeconds} sec\n" +
                $"Max request longevity={m.MaxRequestLongevity.TotalSeconds} sec\n" +
                $"Exceptions count={m.Exceptions.Count}\n" +
                $"Exceptions summary={CreateExceptionsSummary(m.Exceptions)}"));
        }

        private static string CreateExceptionsSummary(IEnumerable<Exception> exceptions)
        {
            var summary = string.Join(". ", exceptions.Select(ex => ex.ToString()));
            return string.IsNullOrWhiteSpace(summary)
                ? "None"
                : summary;
        }
    }
}