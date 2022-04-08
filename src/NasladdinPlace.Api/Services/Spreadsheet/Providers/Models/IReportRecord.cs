using System.Collections.Generic;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public interface IReportRecord
    {
        IEnumerable<string> GetFieldsNames();
    }
}
