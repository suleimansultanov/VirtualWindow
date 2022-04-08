using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Core
{
    public interface IDiagnostics
    {
        event EventHandler<DiagnosticsReport> DiagnosticsCompleted;

        Task RunAsync();
    }
}