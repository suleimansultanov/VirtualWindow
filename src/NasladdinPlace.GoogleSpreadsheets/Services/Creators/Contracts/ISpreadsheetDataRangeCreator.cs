namespace NasladdinPlace.Spreadsheets.Services.Creators.Contracts
{
    public interface ISpreadsheetDataRangeCreator
    {
        string Create(string title, int? columnsCount = null);
    }
}