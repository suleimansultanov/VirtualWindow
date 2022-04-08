using NasladdinPlace.Api.StressTesting.Models;

namespace NasladdinPlace.Api.StressTesting.Args.Contracts
{
    public interface IArgsReader
    {
        bool TryReadConfig(string[] args, out StressTestingConfig config);
    }
}