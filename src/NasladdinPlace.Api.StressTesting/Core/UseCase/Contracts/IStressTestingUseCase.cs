using System.Threading.Tasks;
using NasladdinPlace.Api.StressTesting.Core.Metrics;

namespace NasladdinPlace.Api.StressTesting.Core.UseCase.Contracts
{
    public interface IStressTestingUseCase
    {
        Task<IReadOnlyStressTestingMetrics> StressTestAsync();
    }
}