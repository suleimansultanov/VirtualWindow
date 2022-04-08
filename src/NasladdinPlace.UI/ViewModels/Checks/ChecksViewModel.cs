using NasladdinPlace.Core.Services.Check.Detailed.Models;
using NasladdinPlace.UI.Dtos.ShoppingStatistics;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class ChecksViewModel
    {
        public IPaginatedReport CurrentReport { get; set; }
        public PaginatedReportDto<DetailedCheck> DetaledChecks { get; set; }
        public Criteria Criteria { get; set; }
    }
}
