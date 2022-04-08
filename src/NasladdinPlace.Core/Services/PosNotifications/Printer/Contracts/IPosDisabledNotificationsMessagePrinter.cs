using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.PosNotifications.Printer.Contracts
{
    public interface IPosDisabledNotificationsMessagePrinter
    {
        string Print(IEnumerable<Core.Models.Pos> posDisabledNotificationsInfos);
    }
}