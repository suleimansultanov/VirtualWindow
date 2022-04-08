using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class FiscalizationInfoErrorsBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public FiscalizationInfoErrorsBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel): base(unitOfWorkFactory, configurationModel)
        {
        }
        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                
                var fiscalizationInfoErrorsDateTime = DateTime.UtcNow.AddHours(-_configurationModel.UnpaidPurchaseExpirationHours);
                var fiscalizationInfoTotalErrorsStatisticsLink = _configurationModel.FiscalizationInfoTotalErrorsStatisticsLink;
                var fiscalizationInfoDailyErrorsStatisticsLink = _configurationModel.FiscalizationInfoDailyErrorsStatisticsLink;

                var fiscalizationInfoErrors = 
                    await unitOfWork.PosOperations.GetFiscalizationErrorsInPurchaseModeIncludingPosOperationTransactionsSinceDateTimeAsync(
                        fiscalizationInfoErrorsDateTime);
                var fiscalizationErrorsDailyCount = fiscalizationInfoErrors.Count(po =>
                    po.DatePaid >= reportDateTimeRange.Start && po.DatePaid <= reportDateTimeRange.End);

                return new FiscalizationInfoErrorsContent(
                    fiscalizationInfoErrors.Count,
                    fiscalizationErrorsDailyCount,
                    fiscalizationInfoTotalErrorsStatisticsLink,
                    fiscalizationInfoDailyErrorsStatisticsLink);
            }
        }
    }
}
