using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.OverdueGoods.Checker;
using NasladdinPlace.Reports.DailyReports.Contracts;

namespace NasladdinPlace.Reports.DailyReports
{
    public class OverdueGoodsReport : IReport
    {
        private readonly IOverdueGoodsChecker _overdueGoodsChecker;

        public OverdueGoodsReport(IOverdueGoodsChecker overdueGoodsChecker)
        {
            if (overdueGoodsChecker == null)
                throw new ArgumentNullException(nameof(overdueGoodsChecker));

            _overdueGoodsChecker = overdueGoodsChecker;
        }

        public Task ExecuteAsync()
        {
            return _overdueGoodsChecker.CheckAsync();
        }
    }
}