using System;

namespace NasladdinPlace.Api.Services.Spreadsheet.Models
{
    public class SpreadsheetsUploadingTaskParameters
    {
        public TimeSpan RetryDelay { get; }
        public int PermittedRetryCount { get; }
        public TimeSpan ReportDataExportingPeriodInDays { get; }

        public SpreadsheetsUploadingTaskParameters(TimeSpan retryDelay, int permittedRetryCount, int reportDataExportingPeriodInDays)
        {
            if (retryDelay.Minutes < 0)
                throw new ArgumentOutOfRangeException(nameof(retryDelay), retryDelay,
                    "retryDelay must be greater than zero.");
            if (permittedRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(permittedRetryCount), permittedRetryCount,
                    "permittedRetryCount must be greater than zero.");
            if (reportDataExportingPeriodInDays < 0)
                throw new ArgumentOutOfRangeException(nameof(reportDataExportingPeriodInDays), reportDataExportingPeriodInDays,
                    "reportDataExportingPeriodInDays must be greater than zero.");

            RetryDelay = retryDelay;
            PermittedRetryCount = permittedRetryCount;
            ReportDataExportingPeriodInDays = TimeSpan.FromDays(reportDataExportingPeriodInDays);
        }
    }
}