using System;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base
{
    public abstract class BaseContent
    {
        public abstract string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange);

        protected string GetMoscowDateTimeFilter(DateTime dateTime)
        {
            return UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime).ToDynamicFilterDateFormat();
        }
    }
}