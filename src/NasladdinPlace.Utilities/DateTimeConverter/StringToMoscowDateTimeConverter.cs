using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public class StringToMoscowDateTimeConverter  : IStringToDateTimeConverter
    {
        private readonly IStringToDateTimeConverter _utcStringToDateTimeConverter;

        public StringToMoscowDateTimeConverter(IStringToDateTimeConverter utcStringToDateTimeConverter)
        {
            _utcStringToDateTimeConverter = utcStringToDateTimeConverter;
        }

        public bool Convert(string dateTimeString, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            
            if (!_utcStringToDateTimeConverter.Convert(dateTimeString, out var utcDateTime))
                return false;

            dateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(utcDateTime);
            
            return true;
        }
    }
}
