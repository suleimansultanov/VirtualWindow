using System.Collections.Generic;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Reports.DailyReports.Contracts
{
    public interface IDailyReportsExecutorsGenerator
    {
        IEnumerable<IReport> GetReports();
        IReport GetReport(ReportType reportType);
    }
}