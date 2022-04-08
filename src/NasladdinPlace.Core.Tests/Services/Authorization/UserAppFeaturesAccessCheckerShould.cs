using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Services.Authorization;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.Authorization
{
    public class UserAppFeaturesAccessCheckerShould
    {
        private readonly IAccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;
        private readonly Mock<IUserAccessGroupMembershipManager> _mockUserAccessGroupMembershipManager;

        public UserAppFeaturesAccessCheckerShould()
        {
            _accessGroupAppFeaturesAccessManager = new AccessGroupAppFeaturesAccessManager(new FakeUnitOfWorkFactory());
            _mockUserAccessGroupMembershipManager = new Mock<IUserAccessGroupMembershipManager>();
        }
        
        [Fact]
        public void ReturnAuthorizedStatusWhenAUserHasAccessToAFeature()
        {
            var userAppFeaturesAccessManager = new UserAppFeaturesAccessChecker(
                _accessGroupAppFeaturesAccessManager,
                _mockUserAccessGroupMembershipManager.Object
            );
            
            const int userId = 1;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            _mockUserAccessGroupMembershipManager
                .Setup(m => m.GetAccessGroupsInWhichUserIsAMemberAsync(userId))
                .Returns(Task.FromResult(new List<AccessGroup> { AccessGroup.Admin }.AsEnumerable()));

            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(AccessGroup.Admin, feature).Wait();

            var hasAccess = userAppFeaturesAccessManager.IsAccessToFeatureGrantedAsync(userId, feature).Result;
            hasAccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnUnauthorizedStatusWhenAUserDoesNotHaveAccessToAFeature()
        {
            var userAppFeaturesAccessManager = new UserAppFeaturesAccessChecker(
                _accessGroupAppFeaturesAccessManager,
                _mockUserAccessGroupMembershipManager.Object
            );
            
            const int userId = 1;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            var hasAccess = userAppFeaturesAccessManager.IsAccessToFeatureGrantedAsync(userId, feature).Result;
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void ReturnUnauthorizedResultWhenAUserDoesNotBelongToAccessGroupThatHasAccessToAFeature()
        {
            var userAppFeaturesAccessManager = new UserAppFeaturesAccessChecker(
                _accessGroupAppFeaturesAccessManager,
                _mockUserAccessGroupMembershipManager.Object
            );
            
            const int userId = 1;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            _mockUserAccessGroupMembershipManager
                .Setup(m => m.GetAccessGroupsInWhichUserIsAMemberAsync(userId))
                .Returns(Task.FromResult(Enumerable.Empty<AccessGroup>()));

            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(AccessGroup.Admin, feature);

            var hasAccess = userAppFeaturesAccessManager.IsAccessToFeatureGrantedAsync(userId, feature).Result;
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void ReturnUnauthorizedResultWhenAUserBelongsToAccessGroupThatDoesNotHaveAccessToAFeature()
        {
            var userAppFeaturesAccessManager = new UserAppFeaturesAccessChecker(
                _accessGroupAppFeaturesAccessManager,
                _mockUserAccessGroupMembershipManager.Object
            );
            
            const int userId = 1;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            _mockUserAccessGroupMembershipManager
                .Setup(m => m.GetAccessGroupsInWhichUserIsAMemberAsync(userId))
                .Returns(Task.FromResult(new List<AccessGroup> { AccessGroup.Admin }.AsEnumerable()));

            var hasAccess = userAppFeaturesAccessManager.IsAccessToFeatureGrantedAsync(userId, feature).Result;
            hasAccess.Should().BeFalse();
        }
    }
}