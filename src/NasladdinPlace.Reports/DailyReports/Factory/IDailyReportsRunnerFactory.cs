using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.Runner;

namespace NasladdinPlace.Reports.DailyReports.Factory
{
    public interface IDailyReportsRunnerFactory
    {
        IReportsRunner Create();
        IReportsRunner Create(ReportType reportType);
    }
}
