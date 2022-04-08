using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NasladdinPlace.Core.Services.Authorization;
using NasladdinPlace.Core.Services.Authorization.Models;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.Authorization
{
    public class AccessGroupAppFeaturesAccessManagerShould
    {
        private readonly AccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;

        public AccessGroupAppFeaturesAccessManagerShould()
        {
            _accessGroupAppFeaturesAccessManager = new AccessGroupAppFeaturesAccessManager(new FakeUnitOfWorkFactory());
        }

        [Fact]
        public void GrantAnAccessGroupAccessToAFeature()
        {
            const AccessGroup accessGroup = AccessGroup.Admin;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;
            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(accessGroup, feature).Wait();

            var hasAccess = _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(accessGroup, feature).Result;
            hasAccess.Should().BeTrue();
        }

        [Fact]
        public void RestrictAnAccessGroupAccessToAFeature()
        {
            const AccessGroup accessGroup = AccessGroup.Admin;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;
            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(accessGroup, feature).Wait();

            _accessGroupAppFeaturesAccessManager.RestrictAccessAsync(accessGroup, feature).Wait();

            var hasAccess = _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(accessGroup, feature).Result;
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void ReturnUnauthorizedResultWhenAccessGroupsPermissionsAreNotSpecified()
        {
            const AccessGroup accessGroup = AccessGroup.Admin;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            var hasAccess = _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(accessGroup, feature).Result;
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void ReturnUnauthorizedResultForTheFirstOfTwoAccessGroupsWhenOnlyTheSecondHasAccessToAFeature()
        {
            const AccessGroup firstAccessGroup = AccessGroup.Admin;
            const AccessGroup secondAccessGroup = AccessGroup.Logistician;
            const AppFeature feature = AppFeature.AllowedPosMode_CreateOrDelete;

            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(secondAccessGroup, feature).Wait();

            var hasAccess = _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(firstAccessGroup, feature).Result;
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void RestrictAnAccessGroupAccessOnlyToAGivenFeature()
        {
            const AccessGroup accessGroup = AccessGroup.Admin;
            const AppFeature firstFeature = AppFeature.AllowedPosMode_CreateOrDelete;
            const AppFeature secondFeature = AppFeature.AllowedPosMode_Read;

            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(accessGroup, firstFeature).Wait();
            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(accessGroup, secondFeature).Wait();

            _accessGroupAppFeaturesAccessManager.RestrictAccessAsync(accessGroup, firstFeature).Wait();

            var hasAccessToTheFirstFeature =
                _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(accessGroup, firstFeature).Result;
            var hasAccessToTheSecondFeature =
                _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(accessGroup, secondFeature).Result;

            hasAccessToTheFirstFeature.Should().BeFalse();
            hasAccessToTheSecondFeature.Should().BeTrue();
        }

        [Fact]
        public void ReturnAuthorizedResultForAnyGivenAccessGroupIfItHasAccessToAFeature()
        {
            const AccessGroup adminGroup = AccessGroup.Admin;
            const AccessGroup logisticianGroup = AccessGroup.Logistician;
            const AppFeature appFeature = AppFeature.AllowedPosMode_CreateOrDelete;

            var accessGroups = new List<AccessGroup>
            {
                adminGroup,
                logisticianGroup
            }.AsEnumerable();

            _accessGroupAppFeaturesAccessManager.GrantAccessAsync(adminGroup, appFeature).Wait();

            var hasAccess =
                _accessGroupAppFeaturesAccessManager.IsAccessGrantedForAnyAccessGroupAsync(accessGroups, appFeature).Result;

            hasAccess.Should().BeTrue();
        }
    }
}