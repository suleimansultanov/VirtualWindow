using System.Threading.Tasks;
using NasladdinPlace.Api.StressTesting.Models;

namespace NasladdinPlace.Api.StressTesting.Core
{
    public interface IStressTester
    {
        Task<StressTestingReport> RunAsync();
    }
}