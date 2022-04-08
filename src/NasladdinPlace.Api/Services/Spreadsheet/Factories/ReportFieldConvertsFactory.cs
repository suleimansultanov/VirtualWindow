using NasladdinPlace.Api.Services.Spreadsheet.Converters;
using NasladdinPlace.Api.Services.Spreadsheet.Enums;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories
{
    public class ReportFieldConvertsFactory : IReportFieldConvertsFactory
    {
        public IReportFieldConverter Create(ReportFieldConverterType reportConverter)
        {
            switch (reportConverter)
            {
                case ReportFieldConverterType.Boolean:
                    return new BooleanReportFieldConverter();
                case ReportFieldConverterType.DateTime:
                    return new DateTimeReportFieldConverter();
                default:
                    return default(IReportFieldConverter);
            }
        }
    }
}
