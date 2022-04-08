using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics
{
    public class DailyStatisticsBuilder : IDailyStatisticsBuilder
    {
        private readonly IEnumerable<IDailyStatisticsContentBuilder> _contentBuilders;
        private readonly string _adminPageBaseUrl;

        public DailyStatisticsBuilder(IEnumerable<IDailyStatisticsContentBuilder> contentBuilders, string adminPageBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(adminPageBaseUrl))
                throw new ArgumentNullException(nameof(adminPageBaseUrl), "Admin page url can not be null or empty");
            if (contentBuilders == null || !contentBuilders.Any())
                throw new ArgumentNullException(nameof(contentBuilders), "Content builders can not be null or empty");

            _contentBuilders = contentBuilders;
            _adminPageBaseUrl = adminPageBaseUrl;
        }

        public async Task<string> BuildContentAsync(DateTimeRange reportDateRange)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(reportDateRange.End);
            var shortMoscowDateTimeAsString = SharedDateTimeConverter.ConvertDatePartToString(moscowDateTime);

            var reportData = new StringBuilder();
            reportData.Append($"Ежедневная статистика {shortMoscowDateTimeAsString}");
            reportData.Append($"{Environment.NewLine}Daily");

            foreach (var builder in _contentBuilders)
            {
                var builtContent = await builder.BuildContentWithLinkAsync(reportDateRange);
                
                reportData.Append(Environment.NewLine);
                reportData.Append(builtContent.BuildReportAsString(_adminPageBaseUrl, reportDateRange));
            }

            return reportData.ToString();
        }
    }
}
