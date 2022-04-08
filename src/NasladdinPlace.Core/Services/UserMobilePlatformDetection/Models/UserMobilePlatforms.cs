using System.Collections.Generic;
using System.Collections.Immutable;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.UserMobilePlatformDetection.Models
{
    public class UserMobilePlatforms
    {
        public ApplicationUser User { get; }
        public ICollection<MobilePlatform> MobilePlatforms { get; }

        public UserMobilePlatforms(ApplicationUser user, IEnumerable<MobilePlatform> mobilePlatforms)
        {
            User = user;
            MobilePlatforms = mobilePlatforms.ToImmutableList();
        }
    }
}