using NasladdinPlace.Api.Services.Spreadsheet.Enums;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts
{
    public interface IPurchaseReportRecordFactory
    {
        PurchaseReportRecord CreateReportRecordOrNull(ReportRecordPurchaseType purchaseType, DetailedCheck check,
            DetailedCheckGood checkGood,
            decimal checkItemBonus);
    }
}