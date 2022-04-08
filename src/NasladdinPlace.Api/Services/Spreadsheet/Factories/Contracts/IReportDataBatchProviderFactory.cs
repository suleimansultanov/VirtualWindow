using System;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts
{
    public interface IReportDataBatchProviderFactory
    {
        IReportDataBatchProvider Create(ReportType type, TimeSpan reportDataExportingPeriodInDays);
    }
}
