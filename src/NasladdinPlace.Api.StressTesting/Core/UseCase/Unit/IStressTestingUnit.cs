using System.Threading.Tasks;
using NasladdinPlace.Api.StressTesting.Core.Metrics;

namespace NasladdinPlace.Api.StressTesting.Core.UseCase.Unit
{
    public interface IStressTestingUnit
    {
        Task<IReadOnlyStressTestingMetrics> StressTestAsync();
    }
}