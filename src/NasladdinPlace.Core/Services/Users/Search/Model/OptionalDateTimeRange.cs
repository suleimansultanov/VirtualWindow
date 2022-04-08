using System;

namespace NasladdinPlace.Core.Services.Users.Search.Model
{
    public struct OptionalDateTimeRange
    {
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }

        public bool HasValue =>  From.HasValue && Until.HasValue;
    }
}
