using NasladdinPlace.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Services.PosNotifications.Printer.Contracts;

namespace NasladdinPlace.Core.Services.PosNotifications.Printer
{
    public class PosDisabledNotificationsRussianMessagePrinter : IPosDisabledNotificationsMessagePrinter
    {
        public string Print(IEnumerable<Core.Models.Pos> posDisabledNotificationsInfos)
        {
            var posAbbreviatedNameWithDisabledNotifications = posDisabledNotificationsInfos.Select(pos => pos.AbbreviatedName).ToList();

            return
                $"{Emoji.Clipboard} Оповещения {string.Join(",", posAbbreviatedNameWithDisabledNotifications)} отключены.";
        }
    }
}