using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation
{
    public interface IDiagnosticsStepPreparer
    {
        Task PrepareAsync(DiagnosticsContext diagnosticsContext);
    }
}