using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace NasladdinPlace.Spreadsheets.Services.Formatters.Contracts
{
    public interface ISpreadsheetCellFormatter
    {
        IList<CellData> GetCellFormats(object type);
    }
}