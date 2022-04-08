using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Reports.DailyReports.Contracts;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.DateTimeRange;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics
{
    public class DailyStatisticsReport : IReport
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly DailyStatisticsConfigurationModel _configurationModel;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;

        public DailyStatisticsReport(IUnitOfWorkFactory unitOfWorkFactory,
            ITelegramChannelMessageSender telegramChannelMessageSender,
            DailyStatisticsConfigurationModel configurationModel)
        {
            if (configurationModel == null)
                throw new ArgumentNullException(nameof(configurationModel));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (telegramChannelMessageSender == null)
                throw new ArgumentNullException(nameof(telegramChannelMessageSender));

            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationModel = configurationModel;
            _telegramChannelMessageSender = telegramChannelMessageSender;
        }

        public async Task ExecuteAsync()
        {
            var dailyStatisticsBuilder = GetDailyStatisticsBuilder();
            var reportUtcDateTimeRange =
                UtcMoscowDateTimeRangeUtilities.ComputeMoscowDateTimeRangeForTimespanDaysAgo(_configurationModel.PeriodRange, _configurationModel.DaysAgo);

            var dailyStatisticsContent = await dailyStatisticsBuilder.BuildContentAsync(reportUtcDateTimeRange);

            if (string.IsNullOrEmpty(dailyStatisticsContent))
                return;

            await _telegramChannelMessageSender.SendAsync(dailyStatisticsContent);
        }

        private IDailyStatisticsBuilder GetDailyStatisticsBuilder()
        {
            var contentBuilders = new List<IDailyStatisticsContentBuilder>
            {
                new UsersRegistrationsContentBuilder(_unitOfWorkFactory, _configurationModel),
                new PaidPurchasesContentBuilder(_unitOfWorkFactory, _configurationModel),
                new ExpiredLabeledGoodsBuilder(_unitOfWorkFactory, _configurationModel),
                new UnpaidPurchasesContentBuilder(_unitOfWorkFactory, _configurationModel),
                new UnhandledConditionalPurchaseCountBuilder(_unitOfWorkFactory, _configurationModel),
                new PosAbnormalSensorMeasurementContentBuilder(_unitOfWorkFactory, _configurationModel),
                new FiscalizationInfoErrorsBuilder(_unitOfWorkFactory, _configurationModel)
            };

            return new DailyStatisticsBuilder(contentBuilders, _configurationModel.AdminPageBaseUrl);
        }
    }
}
