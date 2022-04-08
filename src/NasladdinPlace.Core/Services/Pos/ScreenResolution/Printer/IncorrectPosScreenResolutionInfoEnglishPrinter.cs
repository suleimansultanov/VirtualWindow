using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Models;
using NasladdinPlace.Core.Services.Printers.Common;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Services.Pos.ScreenResolution.Printer
{
    public class IncorrectPosScreenResolutionInfoEnglishPrinter : IMessagePrinter<IEnumerable<PosScreenResolutionInfo>>
    {
        public string Print(IEnumerable<PosScreenResolutionInfo> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var messageHeader = $"Incorrect screen resolution on POS: ";
            var separator = ", ";

            var commaSeparatedPointsOfSaleWithLastUpdateDateTimeStringBuilder = entities.Aggregate(
                new StringBuilder(),
                (current, next) => current.Append(current.Length == 0 ? "" : separator).Append(
                    $"{next.PosInfo.Name.Trim()} [{ConvertToMoscowDateTimeString(next.ScreenResolution.DateUpdated)}]"));

            return $"{messageHeader} {commaSeparatedPointsOfSaleWithLastUpdateDateTimeStringBuilder}.";
        }

        private static string ConvertToMoscowDateTimeString(DateTime utcDateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(utcDateTime);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }
    }
}
