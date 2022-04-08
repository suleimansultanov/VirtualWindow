using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Core.Models
{
    public class UsersFiltersContext
    {
        public int? UserId { get; set; }
        public DateTime? NotLazyUsersFrom { get; set; }
        public DateTime? NotLazyUsersUntil { get; set; }
        public DateTime? LazyUsersFrom { get; set; }
        public DateTime? LazyUsersUntil { get; set; }
        public UserLazinessIndex? Type { get; set; }
    }
}

