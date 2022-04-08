using System;
using System.Globalization;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Converters
{
    public class DateTimeReportFieldConverter : IReportFieldConverter
    {
        public string Convert(object field)
        {
            if (field == null)
                return string.Empty;
            
            if(!(field is DateTime))
                throw new ArgumentException($"Expected DateTime field type {field}");

            return System.Convert.ToDateTime((DateTime)field).ToString("t", CultureInfo.InvariantCulture);
        }
    }
}
