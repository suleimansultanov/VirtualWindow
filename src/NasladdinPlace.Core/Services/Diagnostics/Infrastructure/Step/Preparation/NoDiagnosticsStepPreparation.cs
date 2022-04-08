using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation
{
    public class NoDiagnosticsStepPreparation : IDiagnosticsStepPreparer
    {
        public Task PrepareAsync(DiagnosticsContext diagnosticsContext)
        {
            return Task.CompletedTask;
        }
    }
}