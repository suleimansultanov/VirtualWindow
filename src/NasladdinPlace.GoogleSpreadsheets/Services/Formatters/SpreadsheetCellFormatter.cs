using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Apis.Sheets.v4.Data;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;

namespace NasladdinPlace.Spreadsheets.Services.Formatters
{
    public class SpreadsheetCellFormatter : ISpreadsheetCellFormatter
    {
        public IList<CellData> GetCellFormats(object type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public;

            return type.GetType()
                .GetFields(bindingFlags)
                .Select(GetCellFormatForField)
                .ToList();
        }

        private CellData GetCellFormatForField(FieldInfo fieldInfo)
        {
            switch (fieldInfo.FieldType.Name)
            {
                case nameof(Int32):
                    return new CellData
                    {
                        UserEnteredFormat = new CellFormat()
                        {
                            NumberFormat = new NumberFormat()
                            {
                                Pattern = "0",
                                Type = "Number"
                            },
                        },
                    };
                case nameof(Decimal):
                    return new CellData
                    {
                        UserEnteredFormat = new CellFormat()
                        {
                            NumberFormat = new NumberFormat()
                            {
                                Pattern = "0.00",
                                Type = "Number"
                            }
                        }
                    };
                case nameof(String):
                    return new CellData
                    {
                        UserEnteredFormat = new CellFormat()
                        {
                            TextFormat = new TextFormat()
                        }
                    };
                default:
                    return new CellData
                    {
                        UserEnteredFormat = new CellFormat()
                        {
                            TextFormat = new TextFormat()
                        }
                    };
            }
        }
    }
}