using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public class UtcMoscowDateTimeConverter
    {
        private const int MoscowUtcDeltaInHours = 3;
        
        private UtcMoscowDateTimeConverter()
        {
           // intentionally left empty 
        }
        
        public static DateTime ConvertToMoscowDateTime(DateTime utcDateTime)
        {
            return utcDateTime == DateTime.MaxValue ? utcDateTime : utcDateTime.AddHours(MoscowUtcDeltaInHours);
        }

        public static DateTime ConvertToUtcDateTime(DateTime moscowDateTime)
        {
            return moscowDateTime == DateTime.MinValue ? moscowDateTime : moscowDateTime.AddHours(-MoscowUtcDeltaInHours);
        }

        public static DateTime MoscowNow => ConvertToMoscowDateTime(DateTime.UtcNow);
    }
}