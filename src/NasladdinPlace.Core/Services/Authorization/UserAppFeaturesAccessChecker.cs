using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Services.Authorization
{
    public class UserAppFeaturesAccessChecker : IUserAppFeaturesAccessChecker
    {
        private readonly IAccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;
        private readonly IUserAccessGroupMembershipManager _userAccessGroupMembershipManager;

        public UserAppFeaturesAccessChecker(
            IAccessGroupAppFeaturesAccessManager accessGroupAppFeaturesAccessManager, 
            IUserAccessGroupMembershipManager userAccessGroupMembershipManager)
        {
            _accessGroupAppFeaturesAccessManager = accessGroupAppFeaturesAccessManager;
            _userAccessGroupMembershipManager = userAccessGroupMembershipManager;
        }

        public async Task<bool> IsAccessToFeatureGrantedAsync(int userId, AppFeature feature)
        {
            var accessGroupsInWhichUserIsAMember = 
                await _userAccessGroupMembershipManager.GetAccessGroupsInWhichUserIsAMemberAsync(userId);

            return await _accessGroupAppFeaturesAccessManager.IsAccessGrantedForAnyAccessGroupAsync(
                accessGroupsInWhichUserIsAMember, feature
            );
        }

        public Task<bool> IsAccessToFeaturesGrantedAsync(int userId, IEnumerable<AppFeature> features)
        {
            throw new System.NotImplementedException();
        }
    }
}