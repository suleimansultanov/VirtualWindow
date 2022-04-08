using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.PushNotifications;
using NasladdinPlace.Core.Services.PushNotifications.Models.Token;
using NasladdinPlace.Core.Services.UserMobilePlatformDetection.Contracts;
using NasladdinPlace.Core.Services.UserMobilePlatformDetection.Models;

namespace NasladdinPlace.Core.Services.UserMobilePlatformDetection
{
    public class UsersMobilePlatformsDetector : IUsersMobilePlatformsDetector
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPushNotificationsService _pushNotificationsService;

        public UsersMobilePlatformsDetector(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPushNotificationsService pushNotificationsService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _pushNotificationsService = pushNotificationsService;
        }
        
        public async Task<IEnumerable<UserMobilePlatforms>> DetectAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var users = await unitOfWork.Users.GetAllIncludingFirebaseTokensAsync();
                
                var usersMobilePlatforms = new Collection<UserMobilePlatforms>();

                foreach (var user in users)
                {
                    var firebaseTokens = user.FirebaseTokens.Select(ft => ft.Token);
                    var tokenInfos = await GetTokenInfos(firebaseTokens);
                    var mobilePlatforms = tokenInfos.Select(ti => ti.MobilePlatform);
                    var userMobilePlatforms = new UserMobilePlatforms(user, mobilePlatforms);
                    usersMobilePlatforms.Add(userMobilePlatforms);
                }

                return usersMobilePlatforms;
            }
        }

        private async Task<ICollection<PushNotificationTokenInfo>> GetTokenInfos(IEnumerable<string> tokens)
        {
            var tokenInfos = new Collection<PushNotificationTokenInfo>();
            
            foreach (var token in tokens)
            {
                var tokenInfoResult = await _pushNotificationsService.GetTokenInfoAsync(token);

                if (tokenInfoResult.Succeeded)
                {
                    tokenInfos.Add(tokenInfoResult.Value);
                }
            }

            return tokenInfos;
        }
    }
}