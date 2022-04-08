using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner
{
    public class NoDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        public Task CleanUpAsync(DiagnosticsContext context)
        {
            return Task.CompletedTask;
        }
    }
}