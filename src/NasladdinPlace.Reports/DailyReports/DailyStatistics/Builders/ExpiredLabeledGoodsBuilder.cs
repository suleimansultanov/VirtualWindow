using NasladdinPlace.Core;
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
    public class ExpiredLabeledGoodsBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public ExpiredLabeledGoodsBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var checkItems = await unitOfWork.CheckItems.FindInPosWithExpirationDateInTimeRangeAsync(reportDateTimeRange);
                var sumOfExpiredLabeledGoods = checkItems.Sum(lg => lg.Price);

                return new ExpiredLabeledGoodsContent(sumOfExpiredLabeledGoods, _configurationModel.BasePurchasesLink);
            }
        }
    }
}