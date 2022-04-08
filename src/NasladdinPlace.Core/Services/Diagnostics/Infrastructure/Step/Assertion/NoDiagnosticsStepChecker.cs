using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion
{
    public class NoDiagnosticsStepChecker : IDiagnosticsStepSuccessChecker
    {
        public Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            return Task.FromResult(Result.Success());
        }
    }
}