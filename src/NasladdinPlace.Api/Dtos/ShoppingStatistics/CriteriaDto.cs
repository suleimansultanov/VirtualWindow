using System;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Api.Dtos.ShoppingStatistics
{
    public class CriteriaDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string DateTimeFrom { get; set; }
        public string DateTimeUntil { get; set; }
        public int? PlantId { get; set; }

        public DateTime UtcDateTimeFromOrMin => 
            SharedDateTimeConverter.ConvertToUtcDateTime(DateTimeFrom, out var moscowDateTime) 
                ? UtcMoscowDateTimeConverter.ConvertToUtcDateTime(moscowDateTime) 
                : DateTime.MinValue;

        public DateTime UtcDateTimeUntilOrMax => 
            SharedDateTimeConverter.ConvertToUtcDateTime(DateTimeUntil, out var moscowDateTime) 
                ? UtcMoscowDateTimeConverter.ConvertToUtcDateTime(moscowDateTime) 
                : DateTime.MaxValue;

        public CriteriaDto()
        {
            Page = 1;
            PageSize = 10;
        }
    }
}
