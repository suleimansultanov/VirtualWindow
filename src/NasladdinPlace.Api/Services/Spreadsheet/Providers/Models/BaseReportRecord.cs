using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Spreadsheets.Extensions;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public class BaseReportRecord
    {
        protected Type Type { get; set; }

        public BaseReportRecord()
        {
            Type = GetType().UnderlyingSystemType;
        }

        public IEnumerable<string> GetFieldsNames()
        {
            return Type.GetFieldsNames().Cast<string>();
        }
    }
}
