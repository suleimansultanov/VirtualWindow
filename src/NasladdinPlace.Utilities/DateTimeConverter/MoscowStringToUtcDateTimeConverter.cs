using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public class MoscowStringToUtcDateTimeConverter : IStringToDateTimeConverter
    {
        private readonly IStringToDateTimeConverter _utcStringToDateTimeConverter;

        public MoscowStringToUtcDateTimeConverter(IStringToDateTimeConverter utcStringToDateTimeConverter)
        {
            _utcStringToDateTimeConverter = utcStringToDateTimeConverter;
        }

        public bool Convert(string dateTimeString, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            
            if (!_utcStringToDateTimeConverter.Convert(dateTimeString, out var moscowDateTime))
                return false;

            dateTime = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(moscowDateTime);
            
            return true;
        }
    }
}
