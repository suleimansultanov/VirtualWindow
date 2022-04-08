using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory
{
    public interface IDiagnosticsStepFactory
    {
        DiagnosticsStep Create();
    }
}