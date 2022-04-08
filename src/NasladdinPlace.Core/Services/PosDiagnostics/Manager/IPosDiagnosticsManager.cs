using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Manager
{
    public interface IPosDiagnosticsManager
    {
        event EventHandler<List<PosDiagnosticsState>> OnDiagnosticsCompleted;
        Task RunPosDiagnosticsAsync();
    }
}