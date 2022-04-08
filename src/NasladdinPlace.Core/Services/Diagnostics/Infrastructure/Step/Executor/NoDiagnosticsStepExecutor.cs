using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor
{
    public class NoDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        public Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            return Task.FromResult(Result.Success());
        }
    }
}