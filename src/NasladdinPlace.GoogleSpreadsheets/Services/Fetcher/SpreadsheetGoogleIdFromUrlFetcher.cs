using System.Text.RegularExpressions;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;

namespace NasladdinPlace.Spreadsheets.Services.Fetcher
{
    public class SpreadsheetGoogleIdFromUrlFetcher : ISpreadsheetIdFetcher
    {
        public string GetId(string input)
        {
            const string pattern = "([\\w-_]{15,})";
            return Regex.Match(input, pattern).Value;
        }
    }
}
