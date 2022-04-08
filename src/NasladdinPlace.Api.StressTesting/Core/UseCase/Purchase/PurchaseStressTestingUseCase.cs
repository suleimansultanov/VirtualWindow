using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.StressTesting.Core.Metrics;
using NasladdinPlace.Api.StressTesting.Core.UseCase.Contracts;
using NasladdinPlace.Api.StressTesting.Core.UseCase.Unit;
using NasladdinPlace.Dtos.Purchase;

namespace NasladdinPlace.Api.StressTesting.Core.UseCase.Purchase
{
    public class PurchaseStressTestingUseCase : IStressTestingUseCase
    {
        private readonly int _concurrentRequestsNumber;
        private readonly IEnumerable<IStressTestingUnit> _stressTestingUnits;

        public PurchaseStressTestingUseCase(int concurrentRequestsNumber, IRestClient restClient)
        {
            if (restClient == null)
                throw new ArgumentNullException(nameof(restClient));
            
            _concurrentRequestsNumber = concurrentRequestsNumber;
            var purchasesApiRequestExecutor = restClient.ForApi<IPurchasesApi>();
            _stressTestingUnits = new Collection<IStressTestingUnit>
            {
                new RequestStressTestingUnit(
                    stressTestName: "Purchase initiation test",
                    requestExecutor: async () => await purchasesApiRequestExecutor.PerformRequestAsync(
                        api => api.InitiatePurchaseAsync(new PurchaseInitiationRequestDto
                        {
                            QrCode = "f2583e54-80c9-45b8-ba47-020d7d1a7f5c"
                        })
                    )
                ),
                new RequestStressTestingUnit(
                    stressTestName: "Purchase continuation test",
                    requestExecutor: async () => await purchasesApiRequestExecutor.PerformRequestAsync(
                        api => api.ContinuePurchaseAsync()
                    )
                ),
                new RequestStressTestingUnit(
                    stressTestName: "Purchase completion initiation test",
                    requestExecutor: async () => await purchasesApiRequestExecutor.PerformRequestAsync(
                        api => api.InitiatePurchaseCompletionAsync()
                    )
                )
            };
        }
        
        public async Task<IReadOnlyStressTestingMetrics> StressTestAsync()
        {
            var stressTestingUnitsMetrics = new ConcurrentBag<IReadOnlyStressTestingMetrics>();
            foreach (var stressTestingUnit in _stressTestingUnits)
            {
                var stressTestingUnitTasks = new Collection<Task>();
                for (var i = 0; i < _concurrentRequestsNumber; ++i)
                {
                    var stressTestingUnitTask = Task.Run(async () =>
                    {
                        var stressTestingMetrics = await stressTestingUnit.StressTestAsync();
                        stressTestingUnitsMetrics.Add(stressTestingMetrics);
                    });
                    stressTestingUnitTasks.Add(stressTestingUnitTask);
                }
                await Task.WhenAll(stressTestingUnitTasks);

                if (stressTestingUnitsMetrics.Any(m => m.Exceptions.Any())) 
                    return ComputeUseCaseStressTestingMetrics(stressTestingUnitsMetrics.ToImmutableList());
            }

            return ComputeUseCaseStressTestingMetrics(stressTestingUnitsMetrics.ToImmutableList());
        }

        private IReadOnlyStressTestingMetrics ComputeUseCaseStressTestingMetrics(
            ICollection<IReadOnlyStressTestingMetrics> stressTestingUnitsMetrics)
        {
            return new StressTestingMetrics
            {
                ConcurrentRequestsNumber = _concurrentRequestsNumber,
                AverageRequestLongevity = TimeSpan.FromMilliseconds(
                    stressTestingUnitsMetrics.Average(m => m.AverageRequestLongevity.TotalMilliseconds)
                ),
                MaxRequestLongevity = TimeSpan.FromMilliseconds(
                    stressTestingUnitsMetrics.Max(m => m.MaxRequestLongevity.TotalMilliseconds)
                ),
                MinRequestLongevity = TimeSpan.FromMilliseconds(
                    stressTestingUnitsMetrics.Min(m => m.MinRequestLongevity.TotalMilliseconds)
                ),
                Exceptions = stressTestingUnitsMetrics.SelectMany(m => m.Exceptions).ToImmutableList(),
                StressTestName = nameof(PurchaseStressTestingUseCase)
            };
        }
    }
}