using System;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Converters
{
    public class BooleanReportFieldConverter : IReportFieldConverter
    {
        private const string Yes = "Да";
        private const string No = "Нет";

        public string Convert(object field)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));

            if(!(field is bool))
                throw new ArgumentException($"Expected boolean type of field: {nameof(field)}");

            return (bool) field ? Yes : No;
        }
    }
}
