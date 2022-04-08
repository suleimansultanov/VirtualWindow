using NasladdinPlace.Core;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class UnhandledConditionalPurchaseCountBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public UnhandledConditionalPurchaseCountBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var purchasesSearchEndTime = DateTime.UtcNow.AddHours(-_configurationModel.UnpaidPurchaseExpirationHours);
                var unhandledConditionalPurchasesCountStatisticsLink = _configurationModel.DailyUnhandledConditionalPurchasesCountLink;

                var unhandledConditionalPurchasesCollection =
                    await unitOfWork.PosOperations.GetUnhandledConditionalBeforeTimeAsync(purchasesSearchEndTime);

                return new UnhandledConditionalPurchaseCountContent(unhandledConditionalPurchasesCollection.Count,
                    unhandledConditionalPurchasesCountStatisticsLink);
            }
        }
    }
}
