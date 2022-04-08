using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces
{
    public interface IDailyStatisticsBuilder
    {
        Task<string> BuildContentAsync(DateTimeRange reportDateRange);
    }
}
