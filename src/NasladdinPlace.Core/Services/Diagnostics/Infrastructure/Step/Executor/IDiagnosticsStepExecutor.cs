using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor
{
    public interface IDiagnosticsStepExecutor
    {
        Task<Result> ExecuteAsync(DiagnosticsContext context);
    }
}