using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion
{
    public interface IDiagnosticsStepSuccessChecker
    {
        Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context);
    }
}