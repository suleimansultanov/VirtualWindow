using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories
{
    public class ReportDataBatchProviderFactory : IReportDataBatchProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ReportDataBatchProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IReportDataBatchProvider Create(ReportType type, TimeSpan reportDataExportingPeriodInDays)
        {
            var scope = _serviceProvider.CreateScope();

            switch (type)
            {
                case ReportType.DailyPurchaseStatistics:
                    return new PurchaseReportDataBatchProvider(scope.ServiceProvider, reportDataExportingPeriodInDays);
                case ReportType.PointsOfSaleContent:
                    return new PointsOfSaleContentReportDataBatchProvider(scope.ServiceProvider);
                case ReportType.GoodsMovingInfo:
                    return new GoodsMovingReportDataBatchProvider(scope.ServiceProvider);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип отчета");
            }
        }
    }
}
