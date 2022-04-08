using NasladdinPlace.Core;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class UsersRegistrationsContentBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public UsersRegistrationsContentBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var newUsers = await unitOfWork.Users.GetNewInTimeRangeAsync(reportDateTimeRange);

                var reportDateLazyUsersCount =
                    (await unitOfWork.Users.GetLazyInDateRangeAsync(reportDateTimeRange, _configurationModel.UserLazyDaysCount)).Count;

                return new UsersRegistrationsContent(newUsers.Count, reportDateLazyUsersCount,
                    _configurationModel.UserLazyDaysCount, _configurationModel.UsersLazyLink,
                    _configurationModel.UsersNotLazyLink);
            }
        }
    }
}
