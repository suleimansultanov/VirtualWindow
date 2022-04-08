using NasladdinPlace.Spreadsheets.Services.Spreadsheets.Contracts;

namespace NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet.Contracts
{
    public interface IGoogleSpreadsheetServiceFactory
    {
        IGoogleSpreadsheetService Create();
    }
}
