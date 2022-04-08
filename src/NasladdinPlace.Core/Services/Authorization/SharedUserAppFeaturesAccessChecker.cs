using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Services.Authorization
{
    public class SharedUserAppFeaturesAccessChecker : IUserAppFeaturesAccessChecker
    {
        private static readonly object UserAppFeaturesAccessCheckerLock = new object();

        private static IUserAppFeaturesAccessChecker _userAppFeaturesAccessChecker;
        
        public static void Initialize(IUserAppFeaturesAccessChecker userAppFeaturesAccessChecker)
        {
            Instance = userAppFeaturesAccessChecker;
        }
        
        public Task<bool> IsAccessToFeatureGrantedAsync(int userId, AppFeature feature)
        {
            return Instance.IsAccessToFeatureGrantedAsync(userId, feature);
        }

        public static IUserAppFeaturesAccessChecker Instance
        {
            get
            {
                lock (UserAppFeaturesAccessCheckerLock)
                {
                    return _userAppFeaturesAccessChecker;
                }
            }
            private set
            {
                lock (UserAppFeaturesAccessCheckerLock)
                {
                    _userAppFeaturesAccessChecker = value;
                }
            }
        }
    }
}