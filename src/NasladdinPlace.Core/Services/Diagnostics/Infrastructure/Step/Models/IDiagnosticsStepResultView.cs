using System;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models
{
    public interface IDiagnosticsStepResultView
    {
        bool Succeeded { get; }
        Guid DiagnosticsStepId { get; }
        DiagnosticsStepInfo DiagnosticsStepInfo { get; }
        ErrorBundle ErrorBundle { get; }
        bool HasError { get; }
    }
}