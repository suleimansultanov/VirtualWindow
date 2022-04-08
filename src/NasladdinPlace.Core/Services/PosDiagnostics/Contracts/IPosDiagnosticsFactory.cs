using NasladdinPlace.Core.Services.PosDiagnostics.Models;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Contracts
{
    public interface IPosDiagnosticsFactory
    {
        IPosDiagnostics Create(PosDiagnosticsType posDiagnosticsType, PosDiagnosticsContext context);
    }
}