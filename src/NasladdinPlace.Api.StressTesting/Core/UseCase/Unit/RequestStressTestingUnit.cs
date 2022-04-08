using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Api.StressTesting.Core.Metrics;

namespace NasladdinPlace.Api.StressTesting.Core.UseCase.Unit
{
    public class RequestStressTestingUnit : IStressTestingUnit
    {
        private readonly Func<Task<BaseRequestResponse>> _requestExecutor;
        private readonly Stopwatch _stopwatch;

        public RequestStressTestingUnit(string stressTestName, Func<Task<BaseRequestResponse>> requestExecutor)
        {
            if (string.IsNullOrWhiteSpace(stressTestName))
                throw new ArgumentNullException(nameof(stressTestName));
            if (requestExecutor == null)
                throw new ArgumentNullException(nameof(requestExecutor));
            
            _requestExecutor = requestExecutor;
            _stopwatch = new Stopwatch();
        }
        
        public async Task<IReadOnlyStressTestingMetrics> StressTestAsync()
        {
            _stopwatch.Start();
            var requestResponse = await _requestExecutor.Invoke();
            _stopwatch.Stop();

            var requestExecutionTime = _stopwatch.Elapsed;

            var stressTestingMetrics = new StressTestingMetrics
            {
                AverageRequestLongevity = requestExecutionTime,
                MaxRequestLongevity = requestExecutionTime,
                MinRequestLongevity = requestExecutionTime,
                ConcurrentRequestsNumber = 1
            };
            
            if (!requestResponse.IsRequestSuccessful)
                stressTestingMetrics.Exceptions.Add(requestResponse.Exception);

            return stressTestingMetrics;
        }
    }
}