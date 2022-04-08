using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Reports.DailyReports.Contracts;

namespace NasladdinPlace.Reports.DailyReports
{
    public class DailyPurchaseReport : IReport
    {
        private readonly ISpreadsheetsUploader _spreadsheetsUploader;

        public DailyPurchaseReport(ISpreadsheetsUploader spreadsheetsUploader)
        {
            if (spreadsheetsUploader == null)
                throw new ArgumentNullException(nameof(spreadsheetsUploader));

            _spreadsheetsUploader = spreadsheetsUploader;
        }

        public Task ExecuteAsync()
        {
            return _spreadsheetsUploader.UploadAsync(Core.Enums.ReportType.DailyPurchaseStatistics);
        }
    }
}