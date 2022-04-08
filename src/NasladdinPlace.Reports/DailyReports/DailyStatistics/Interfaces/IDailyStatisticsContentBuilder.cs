using System.Threading.Tasks;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces
{
    public interface IDailyStatisticsContentBuilder
    {
        Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange);
    }
}
