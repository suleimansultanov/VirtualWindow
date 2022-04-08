using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    internal interface IDateTimeToStringConverter
    {
        string ConvertDatePart(DateTime dateTime);
        string ConvertTimePart(DateTime dateTime);
        string ConvertDateHourMinuteParts(DateTime dateTime);
        string Convert(DateTime dateTime);
    }
}
