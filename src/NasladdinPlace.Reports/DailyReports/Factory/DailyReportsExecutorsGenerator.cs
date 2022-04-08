using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.OverdueGoods.Checker;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Reports.DailyReports.Contracts;
using NasladdinPlace.Reports.DailyReports.DailyStatistics;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;

namespace NasladdinPlace.Reports.DailyReports.Factory
{
    public class DailyReportsExecutorsGenerator : IDailyReportsExecutorsGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DailyStatisticsConfigurationModel _configurationModel;

        public DailyReportsExecutorsGenerator(
            IServiceProvider serviceProvider,
            DailyStatisticsConfigurationModel configurationModel)
        {
            if (configurationModel == null)
                throw new ArgumentNullException(nameof(configurationModel));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _configurationModel = configurationModel;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IReport> GetReports()
        {
            return new[]
            {
                GetDailyPurchaseReport(),
                GetPointsOfSaleContentReport(),
                GetDailyGoodsMovingReport(),
                GetDailyStatisticsReport(),
                GetOverdueGoodsReport()
            };
        }

        public IReport GetReport(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.DailyPurchaseStatistics:
                    return GetDailyPurchaseReport();
                case ReportType.GoodsMovingInfo:
                    return GetDailyGoodsMovingReport();
                case ReportType.PointsOfSaleContent:
                    return GetPointsOfSaleContentReport();
            }

            throw new ArgumentOutOfRangeException(nameof(reportType), reportType, "Unrecognized report type");
        }

        private IReport GetDailyStatisticsReport()
        {
            var unitOfWorkFactory = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            var telegramChannelMessageSender = _serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();

            return new DailyStatisticsReport(unitOfWorkFactory, telegramChannelMessageSender, _configurationModel);
        }

        private IReport GetOverdueGoodsReport()
        {
            var overdueGoodsChecker = _serviceProvider.GetRequiredService<IOverdueGoodsChecker>();
            return new OverdueGoodsReport(overdueGoodsChecker);
        }

        private IReport GetPointsOfSaleContentReport()
        {
            var spreadsheetsUploader = _serviceProvider.GetRequiredService<ISpreadsheetsUploader>();
            return new PointsOfSaleContentReport(spreadsheetsUploader);
        }

        private IReport GetDailyPurchaseReport()
        {
            var spreadsheetsUploader = _serviceProvider.GetRequiredService<ISpreadsheetsUploader>();
            return new DailyPurchaseReport(spreadsheetsUploader);
        }

        private IReport GetDailyGoodsMovingReport()
        {
            var spreadsheetsUploader = _serviceProvider.GetRequiredService<ISpreadsheetsUploader>();
            return new GoodsMovingReport(spreadsheetsUploader);
        }
    }
}