using System.Collections.Generic;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts
{
    public interface IPurchaseReportRecordsCreator
    {
        IList<IReportRecord> CreateFromDetailedChecks(IEnumerable<DetailedCheck> detailedChecks);
        IList<PurchaseReportRecord> CreateFromDetailedCheck(DetailedCheck check);
    }
}