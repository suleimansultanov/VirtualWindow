using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Authorization
{
    public class AccessGroupAppFeaturesAccessManager : IAccessGroupAppFeaturesAccessManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public AccessGroupAppFeaturesAccessManager(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task GrantAccessAsync(AccessGroup accessGroup, AppFeature feature)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByAccessGroupIncludingPermittedAppFeaturesAsync(accessGroup);
                role.PermitAppFeature(feature);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task GrantAccessAsync(int roleId, int appFeatureId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByIdIncludingPermittedAppFeaturesAsync(roleId);
                role.PermitAppFeature(role.Id, appFeatureId);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> IsAccessGrantedAsync(AccessGroup accessGroup, AppFeature feature)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByAccessGroupIncludingPermittedAppFeaturesAsync(accessGroup);

                return role.IsAppFeaturePermitted(feature);
            }
        }

        public async Task<bool> IsAccessGrantedAsync(int roleId, string appFeatureName)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByIdIncludingPermittedAppFeaturesAsync(roleId);
                var appFeatureRecord = unitOfWork.AppFeatureItems.GetByName(appFeatureName);

                return appFeatureRecord != null && role.IsAppFeaturePermitted(appFeatureRecord.Id);
            }
        }

        public async Task<bool> IsAccessGrantedAsync(int roleId, int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByIdIncludingPermittedAppFeaturesAsync(roleId);

                return role.AssignedPoses.Contains(posId);
            }
        }

        public async Task<bool> IsAccessGrantedForAnyAccessGroupAsync(
            IEnumerable<AccessGroup> accessGroups,
            AppFeature appFeature)
        {
            foreach (var accessGroup in accessGroups)
            {
                if (await IsAccessGrantedAsync(accessGroup, appFeature)) return true;
            }

            return false;
        }

        public async Task RestrictAccessAsync(AccessGroup accessGroup, AppFeature feature)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByAccessGroupIncludingPermittedAppFeaturesAsync(accessGroup);
                role.RestrictAppFeature(feature);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task RestrictAccessAsync(string roleName, int appFeatureId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var role = await unitOfWork.Roles.GetByNameIncludingPermittedAppFeaturesAsync(roleName);
                role.RestrictAppFeature(appFeatureId);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task SavePermissionsAsync(List<Role> roles, List<AppFeatureItem> appFeatures, IFormCollection form)
        {
            foreach (var role in roles)
            {
                var formKey = "allow_" + role.Id;
                var appFeatureNamesToGrantAccess = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                await GrantOrRestrictAccessToRoleAsync(appFeatures, appFeatureNamesToGrantAccess, role);
            }
        }

        private async Task GrantOrRestrictAccessToRoleAsync(IEnumerable<AppFeatureItem> appFeatures, ICollection<string> appFeatureNamesToGrantAccess, Role role)
        {
            foreach (var appFeature in appFeatures)
            {
                var isAllowed = appFeatureNamesToGrantAccess.Contains(appFeature.Name);
                if (isAllowed)
                {
                    if (appFeature.RolePermittedFeatures.FirstOrDefault(x => x.RoleId == role.Id) != null)
                        continue;

                    await GrantAccessAsync(role.Id, appFeature.Id);
                }
                else
                {
                    if (appFeature.RolePermittedFeatures.FirstOrDefault(x => x.RoleId == role.Id) == null)
                        continue;

                    await RestrictAccessAsync(role.Name, appFeature.Id);
                }
            }
        }
    }
}