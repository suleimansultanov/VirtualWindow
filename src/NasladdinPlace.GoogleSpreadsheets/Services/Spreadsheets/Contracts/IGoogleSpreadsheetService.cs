using NasladdinPlace.Spreadsheets.Models;
using NasladdinPlace.Utilities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Spreadsheets.Services.Spreadsheets.Contracts
{
    public interface IGoogleSpreadsheetService
    {
        Task<ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>> CreateTableAsync(string title);
        Task<bool> TryAddSheetAsync(string spreadsheetId, string title);
        Task<ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>> GetSpreadsheetByIdAsync(string id);
        Task<bool> TryFillAsync(UploadingSpreadsheetInfo info);
        Task TryFillWithFormatsAsync(UploadingSpreadsheetInfo info);
        Task<bool> TryClearAsync(UploadingSpreadsheetInfo info);
        Task<bool> TryClearFiltersAsync(UploadingSpreadsheetInfo info);
        Task<ValueResult<IEnumerable<IList<object>>>> ReadAsync(UploadingSpreadsheetInfo info);
        Task<bool> TryAddCellWithUploadingProgressAsync(SpreadsheetUploadingProgressCell cell,
            UploadingSpreadsheetInfo info);
    }
}
