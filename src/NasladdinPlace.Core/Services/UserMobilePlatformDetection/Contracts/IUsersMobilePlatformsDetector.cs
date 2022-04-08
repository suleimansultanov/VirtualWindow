using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.UserMobilePlatformDetection.Models;

namespace NasladdinPlace.Core.Services.UserMobilePlatformDetection.Contracts
{
    public interface IUsersMobilePlatformsDetector
    {
        Task<IEnumerable<UserMobilePlatforms>> DetectAsync();
    }
}