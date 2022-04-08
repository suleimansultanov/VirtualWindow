using NasladdinPlace.Core.Services.Spreadsheet.Contracts;

namespace NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts
{
    public interface ISpreadsheetProvider
    {
        ISpreadsheet Provide(string url);
    }
}
