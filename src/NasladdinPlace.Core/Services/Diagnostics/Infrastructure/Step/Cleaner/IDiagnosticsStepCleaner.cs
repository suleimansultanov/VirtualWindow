using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner
{
    public interface IDiagnosticsStepCleaner
    {
        Task CleanUpAsync(DiagnosticsContext context);
    }
}