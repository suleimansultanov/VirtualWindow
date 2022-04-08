using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using System;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder
{
    public class UsersRegistrationsContentBuilderShould : TestsBase
    {
        private const int DefaultIsUserLazyDaysCountFlag = 30;
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;

        private IDailyStatisticsContentBuilder _contentBuilder;

        public override void SetUp()
        {
            base.SetUp();

            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();

            var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel()
            {
                UserLazyDaysCount = DefaultIsUserLazyDaysCountFlag,
                UsersLazyLink = "{0}/Users?LazyUsersFrom={1}&LazyUsersUntil={2}&Type={3}",
                UsersNotLazyLink = "{0}/Users?NotLazyUsersFrom={1}&NotLazyUsersUntil={2}",
                AdminPageBaseUrl = "http://nursultanapi.nasladdin.club"
            };

            _contentBuilder = new UsersRegistrationsContentBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenUsersTableIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            AssertExpectedUsersRegistrationsBuildContent(utcTimeInterval, 
                expectedAllUsersCount: 0,
                expectedNewUsersCount: 0,
                expectedReportLazyUsersCount: 0);
        }

        [Test]
        public void ReturnExpectedAllUsersCountWhenGivenDateRangeWithoutNewUsers()
        {
            var endUtcDateTime = DateTime.UtcNow.AddDays(-5);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());

            AssertExpectedUsersRegistrationsBuildContent(utcTimeInterval,
                expectedAllUsersCount: 2,
                expectedNewUsersCount: 0,
                expectedReportLazyUsersCount: 0);
        }

        [Test]
        public void ReturnExpectedAllAndNewUsersCountWhenGivenCorrectlyDateRange()
        {
            var endUtcDateTime = DateTime.UtcNow.AddHours(1);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());

            AssertExpectedUsersRegistrationsBuildContent(utcTimeInterval,
                expectedAllUsersCount: 2,
                expectedNewUsersCount: 2,
                expectedReportLazyUsersCount: 0);
        }

        [Test]
        public void ReturnExpectedCountByCorrectlyTimeIntervalAndPosOperationsIsEmptyForUserWithPaymentCardConfirmationCompletion()
        {
            var endUtcDateTime = DateTime.UtcNow.AddMonths(2);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());

            var user = Context.Users.FirstOrDefault(u => u.Id == DefaultUserId);
            user.NotifyPaymentCardConfirmationCompletion();
            Context.SaveChanges();

            AssertExpectedUsersRegistrationsBuildContent(utcTimeInterval,
                expectedAllUsersCount: 2,
                expectedNewUsersCount: 0,
                expectedReportLazyUsersCount: 0);
        }

        [Test]
        public void ReturnExpectedAllFieldsInUsersRegistrationsContentPosOperationsIsNotEmptyForUserWithPaymentCardConfirmationCompletion()
        {
            var endUtcDateTime = DateTime.UtcNow.AddMonths(2);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));

            var user = Context.Users.FirstOrDefault(u => u.Id == DefaultUserId);
            user.NotifyPaymentCardConfirmationCompletion();
            Context.SaveChanges();

            AssertExpectedUsersRegistrationsBuildContent(utcTimeInterval,
                expectedAllUsersCount: 2,
                expectedNewUsersCount: 0,
                expectedReportLazyUsersCount: 0);
        }

        private void AssertExpectedUsersRegistrationsBuildContent(DateTimeRange utcDateTimeRange,
            int expectedAllUsersCount, int expectedNewUsersCount, int expectedReportLazyUsersCount)
        {
            var resultContent =
                (UsersRegistrationsContent)_contentBuilder
                    .BuildContentWithLinkAsync(utcDateTimeRange).Result;

            resultContent.Should().NotBeNull();
            resultContent.NewUsersCount.Should().Be(expectedNewUsersCount);
            resultContent.ReportLazyUsersCount.Should().Be(expectedReportLazyUsersCount);
        }
    }
}