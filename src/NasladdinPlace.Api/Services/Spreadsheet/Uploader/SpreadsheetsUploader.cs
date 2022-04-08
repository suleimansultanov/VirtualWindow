using Google;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Spreadsheet.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Spreadsheets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Spreadsheet.Helpers;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Api.Services.Spreadsheet.Uploader
{
    public class SpreadsheetsUploader : ISpreadsheetsUploader
    {
        public event EventHandler<ReportUploadingError> ErrorHandler;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISpreadsheetProvider _spreadsheetProvider;
        private readonly SpreadsheetsUploadingTaskParameters _spreadsheetsUploadingTaskParameters;
        private readonly IReportDataBatchProviderFactory _dataBatchProviderFactory;
        private readonly ILogger _logger;

        public SpreadsheetsUploader(IUnitOfWorkFactory unitOfWorkFactory,
            ISpreadsheetProvider spreadsheetProvider,
            IReportDataBatchProviderFactory dataBatchProviderFactory,
            SpreadsheetsUploadingTaskParameters spreadsheetsUploadingTaskParameters,
            ILogger logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _spreadsheetProvider = spreadsheetProvider;
            _spreadsheetsUploadingTaskParameters = spreadsheetsUploadingTaskParameters;
            _logger = logger;
            _dataBatchProviderFactory = dataBatchProviderFactory;
        }

        public async Task UploadAsync(ReportType reportType)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var reportUploadingInfo =
                    await unitOfWork.ReportsUploadingInfo.GetReportsUploadingInfoByType(reportType);

                if (reportUploadingInfo == null)
                    return;

                var spreadsheet = _spreadsheetProvider.Provide(reportUploadingInfo.Url);

                try
                {
                    _logger.LogInfo($"Start of uploading {reportUploadingInfo.Type} report");
                    await RetryHelper.RetryOnExceptionAsync(_spreadsheetsUploadingTaskParameters.RetryDelay, 
                        async () =>
                        {
                            await spreadsheet.ClearAsync(reportUploadingInfo.Sheet);
                            await AddColumnsNameAsync(spreadsheet, reportUploadingInfo);
                            await UploadReportDataAsync(spreadsheet, reportUploadingInfo);
                        }, IsKnownGoogleApiException,
                        _spreadsheetsUploadingTaskParameters.PermittedRetryCount);
                    _logger.LogInfo($"End of uploading {reportUploadingInfo.Type} report");
                }
                catch (Exception e)
                {
                    var reportUploadingError = e is GoogleApiException googleApiException
                        ? new ReportUploadingError(googleApiException.Error.Code,
                            googleApiException.Error.Message, reportUploadingInfo.Type.GetDisplayName())
                        : new ReportUploadingError(default(int), e.Message, reportUploadingInfo.Type.GetDisplayName());

                    ErrorHandler?.Invoke(this, reportUploadingError);
                }
            }
        }

        private async Task AddColumnsNameAsync(ISpreadsheet spreadsheet, ReportUploadingInfo report)
        {
            await spreadsheet.FillAsync(new List<IList<object>> {GetColumnsName(report.Type) }, report.Sheet);
        }

        private IList<object> GetColumnsName(ReportType type)
        {
            switch (type)
            {
                case ReportType.DailyPurchaseStatistics:
                    return typeof(PurchaseReportRecord).GetFieldsNames();
                case ReportType.PointsOfSaleContent:
                    return typeof(PosGoodReportRecord).GetFieldsNames();
                case ReportType.GoodsMovingInfo:
                    return typeof(GoodMovingReportRecord).GetFieldsNames();
                default:
                    return new List<object>();
            }
        }

        private async Task UploadReportDataAsync(ISpreadsheet spreadsheet, ReportUploadingInfo reportUploadingInfo)
        {
            var provider = _dataBatchProviderFactory.Create(reportUploadingInfo.Type, _spreadsheetsUploadingTaskParameters.ReportDataExportingPeriodInDays);
            foreach (var recordsAndProgress in provider.Provide(reportUploadingInfo.BatchSize))
            {
                await spreadsheet.FillAsync(recordsAndProgress.Records, reportUploadingInfo.Sheet);
                _logger.LogInfo($"Report {reportUploadingInfo.Type.ToString()} uploaded with {recordsAndProgress.UploadingProgress.UploadingProgressInPercents:P0} progress");
                await spreadsheet.AddProgressNoteAsync(recordsAndProgress.UploadingProgress, recordsAndProgress.TypeOfRecords, reportUploadingInfo.Sheet);
            }
        }

        private static bool IsKnownGoogleApiException(Exception exception)
        {
            if (exception is GoogleApiException googleApiException)
            {
                return new[]
                {
                    (int) HttpStatusCode.InternalServerError,
                    (int) HttpStatusCode.ServiceUnavailable
                }.Contains(googleApiException.Error.Code);
            }

            return false;
        }
    }
}
