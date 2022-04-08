using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public interface IStringToDateTimeConverter
    {
        bool Convert(string dateTimeString, out DateTime dateTime);
    }
}
