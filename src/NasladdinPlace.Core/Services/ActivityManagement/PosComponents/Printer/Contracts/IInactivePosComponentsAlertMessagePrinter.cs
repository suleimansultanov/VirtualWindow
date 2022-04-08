using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer.Contracts
{
    public interface IInactivePosComponentsAlertMessagePrinter
    {
        Task<string> PrintAsync(IEnumerable<PosComponentsInactivityInfo> inactivePosComponentsWithInactiveTime);
    }
}
