using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;

namespace NasladdinPlace.Api.Services.ActivityManagement.PosComponents.InactivityAlertSender.Contracts
{
    public interface IPosComponentsInactivityAlertSender
    {
        Task SendAsync(IEnumerable<PosComponentsInactivityInfo> posDisplayInactivityInfos);
    }
}