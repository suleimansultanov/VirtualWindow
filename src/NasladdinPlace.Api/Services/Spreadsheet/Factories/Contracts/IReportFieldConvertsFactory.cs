using NasladdinPlace.Api.Services.Spreadsheet.Enums;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts
{
    public interface IReportFieldConvertsFactory
    {
        IReportFieldConverter Create(ReportFieldConverterType reportConverter);
    }
}
