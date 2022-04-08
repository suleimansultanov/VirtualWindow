using System;
using System.Collections.Generic;
using System.Text;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;

namespace NasladdinPlace.CheckOnline.Infrastructure.Helpers
{
    public static class DocumentBuilder
    {
        private static readonly Dictionary<int, string> DocumentTitles = new Dictionary<int, string>
        {
            {1000, string.Empty},
            {1054, "Признак расчета"},
            {1020, "Сумма коррекции"},
            {1031, "Наличными"},
            {1081, "Электронными"},
            {1048, string.Empty},
            {1018, "ИНН"},
            {1012, "Дата время"},
            {1037, "РН ККТ"},
            {1009, string.Empty},
            {1187, "Место расчетов"},
            {1040, "ФД"},
            {1041, "ФП"},
        };

        private const string Separator = ":  ";

        public static string BuildCorrectionDocument(List<CheckOnlineFiscalDocumentTag> documentTags)
        {
            var document = new StringBuilder();

            foreach (var tag in documentTags)
            {
                if (!DocumentTitles.ContainsKey(tag.TagId))
                    continue;

                var tagTitle = DocumentTitles[tag.TagId];
                var title = !string.IsNullOrWhiteSpace(tagTitle) 
                                    ? $"{tagTitle.ToUpper()}{Separator}"
                                    : "";

                var value = tag.Value;

                switch (tag.TagType)
                {
                    case "string":
                        document.Append($"{title}{GetStringValue(value)}");
                        break;
                    case "byte":
                        document.Append($"{title}{GetCustomByteValue(tag)}");
                        break;
                    case "money":
                        document.Append($"{title}{GetMoneyValue(value):0.00}");
                        break;
                    case "unixtime":
                        document.Append($"{title}{GetDateTimeValue(value):dd-MM-yyyy HH:mm}");
                        break;
                    case "uint32":
                        document.Append($"{title}{GetIntValue(value)}");
                        break;
                    default:
                        continue;
                }
                document.Append(Environment.NewLine);
            }

            return document.ToString();
        }

        private static string GetStringValue(object objValue)
        {
            return objValue.ToString();
        }

        private static byte GetByteValue(object objValue)
        {
            var stringValue = GetStringValue(objValue);
            return byte.Parse(stringValue);
        }

        private static decimal GetMoneyValue(object objValue)
        {
            var stringValue = GetStringValue(objValue);
            decimal onlineCheckMoneyFormat = decimal.Parse(stringValue);
            return onlineCheckMoneyFormat / 100;
        }

        private static DateTime GetDateTimeValue(object objValue)
        {
            var stringValue = GetStringValue(objValue);
            return DateTime.Parse(stringValue);
        }

        private static int GetIntValue(object objValue)
        {
            var stringValue = GetStringValue(objValue);
            return int.Parse(stringValue);
        }

        private static string GetCustomByteValue(CheckOnlineFiscalDocumentTag tag)
        {
            switch (tag.TagId)
            {
                case 1054:
                    var byteValue = GetByteValue(tag.Value);
                    return byteValue == 1 ? "Приход" : "Расход";
                default:
                    return tag.Value.ToString();            
            }
        }

    }
}
