using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class PaidPurchasesContentBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public PaidPurchasesContentBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperations =
                    await unitOfWork.PosOperations.GetPaidIncludingCheckItemsWhereDatePaidInTimeRangeAsync(reportDateTimeRange);

                var totalPrice = posOperations.Sum(p =>
                        p.CheckItems.Where(ci => ci.Status == CheckItemStatus.Paid)
                            .Sum(ci => ci.PriceWithDiscount) - p.BonusAmount);

                return new PaidPurchasesContent(posOperations.Count, totalPrice, _configurationModel.BasePurchasesLink);
            }
        }
    }
}
