using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PosNotifications.Manager.Contracts
{
    public interface IPosNotificationsManager
    {
        event EventHandler<IEnumerable<Core.Models.Pos>> OnPosNotificationsBecomeDisabled;
        Task FindPosWithDisabledNotificationsAsync();
    }
}