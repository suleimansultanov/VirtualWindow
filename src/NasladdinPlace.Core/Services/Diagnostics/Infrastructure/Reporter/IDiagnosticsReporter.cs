using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Reporter
{
    public interface IDiagnosticsReporter
    {
        Task<string> PrintReportAsync(DiagnosticsResult diagnosticsResult);
    }
}