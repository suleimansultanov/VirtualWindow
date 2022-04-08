using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner
{
    public interface IDiagnosticsRunner
    {
        event EventHandler<EventArgs> DiagnosticsStarted;
        event EventHandler<DiagnosticsResult> DiagnosticsCompleted;
        event EventHandler<DiagnosticsStepInfo> DiagnosticsStepStarted; 
        event EventHandler<DiagnosticsStepResult> DiagnosticsStepCompleted;

        Task RunAsync();
    }
}