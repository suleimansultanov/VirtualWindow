using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;

namespace NasladdinPlace.Spreadsheets.Services.Creators
{
    public class SpreadsheetDataRangeCreator : ISpreadsheetDataRangeCreator
    {
        private const char StartColumnName = 'A';
        private const char EndColumnName = 'Z';

        public string Create(string title, int? columnsCount = null)
        {
            string endColumn = columnsCount.HasValue ? GetEndColumnName(columnsCount.Value) : EndColumnName.ToString();
            return $"\'{title}\'!{StartColumnName}:{endColumn}";
        }

        private string GetEndColumnName(int endColumnIndex)
        {
            var outputColumnName = string.Empty;
            var spreadsheetDefaultLength = EndColumnName - StartColumnName + 1;
            var tempNumber = endColumnIndex;
            while (tempNumber > 0)
            {
                var position = tempNumber % spreadsheetDefaultLength;
                outputColumnName = (position == 0 ? EndColumnName : (char)(StartColumnName + position - 1)) + outputColumnName;
                tempNumber = (tempNumber - 1) / spreadsheetDefaultLength;
            }

            return outputColumnName;
        }
    }
}
