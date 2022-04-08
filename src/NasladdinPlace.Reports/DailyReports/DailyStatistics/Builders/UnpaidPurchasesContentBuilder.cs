using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class UnpaidPurchasesContentBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public UnpaidPurchasesContentBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var unpaidDateTime = DateTime.UtcNow.AddHours(-_configurationModel.UnpaidPurchaseExpirationHours);
                var posOperations =
                    await unitOfWork.PosOperations.GetUnpaidInPurchaseModeIncludingCheckItemsSinceDateTimeAsync(
                        unpaidDateTime);

                var unpaidStatisticsLink = _configurationModel.TotalUnpaidCheckItemsLink;

                var totalUnpaidPrice = posOperations.Sum(p =>
                                            p.CheckItems.Where(ci => ci.Status == CheckItemStatus.Unpaid)
                                            .Sum(ci => ci.PriceWithDiscount));

                var unpaidOperationsCount = posOperations.Count(po =>
                                            po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid));

                return new UnpaidPurchasesContent(unpaidOperationsCount, totalUnpaidPrice, unpaidStatisticsLink, _configurationModel.UnpaidPurchaseExpirationHours);
            }
        }
    }
}
