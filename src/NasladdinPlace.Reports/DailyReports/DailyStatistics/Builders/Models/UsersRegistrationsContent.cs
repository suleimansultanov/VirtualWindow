using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using NasladdinPlace.Utilities.Models;
using System;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class UsersRegistrationsContent : BaseContent
    {
        public int NewUsersCount { get; private set; }
        public int UserLazyDaysCount { get; private set; }
        public int ReportLazyUsersCount { get; private set; }
        public string UsersLazyLink { get; private set; }
        public string UsersNotLazyLink { get; private set; }

        public UsersRegistrationsContent(int newUsersCount,
            int reportLazyUsersCount, int userLazyDaysCount,
            string usersLazyLink,
            string usersNotLazyLink)
        {
            if (string.IsNullOrEmpty(usersLazyLink))
                throw new ArgumentNullException(nameof(usersLazyLink));
            if (string.IsNullOrEmpty(usersNotLazyLink))
                throw new ArgumentNullException(nameof(usersNotLazyLink));

            NewUsersCount = newUsersCount;
            ReportLazyUsersCount = reportLazyUsersCount;
            UserLazyDaysCount = userLazyDaysCount;
            UsersLazyLink = usersLazyLink;
            UsersNotLazyLink = usersNotLazyLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var newUsersFilterUrl = string.Format(UsersNotLazyLink,
                adminPageBaseUrl,
                GetMoscowDateTimeFilter(reportDateRange.Start),
                GetMoscowDateTimeFilter(reportDateRange.End));

            var lazyUsersFilterUrl = string.Format(UsersLazyLink,
                adminPageBaseUrl,
                reportDateRange.Start.ToMoscowDateTimeStringMinusDays(UserLazyDaysCount),
                reportDateRange.End.ToMoscowDateTimeStringMinusDays(UserLazyDaysCount),
                UserLazinessIndex.Lazy);

            return $"[NewUsers: {NewUsersCount}+, ]({newUsersFilterUrl})" +
                   $"[LazyUsers: {ReportLazyUsersCount}+]({lazyUsersFilterUrl})";
        }
    }
}
