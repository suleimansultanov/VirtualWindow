using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.Contracts
{
    public interface IReport
    {
        Task ExecuteAsync();
    }
}