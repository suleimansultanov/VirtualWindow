using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Services.Authorization.Contracts
{
    public interface IAccessGroupAppFeaturesAccessManager
    {
        Task GrantAccessAsync(AccessGroup accessGroup, AppFeature feature);
        Task GrantAccessAsync(int roleId, int appFeatureId);
        Task<bool> IsAccessGrantedAsync(AccessGroup accessGroup, AppFeature feature);
        Task<bool> IsAccessGrantedAsync(int roleId, string appFeatureName);
        Task<bool> IsAccessGrantedAsync(int roleId, int posId);
        Task<bool> IsAccessGrantedForAnyAccessGroupAsync(IEnumerable<AccessGroup> accessGroups, AppFeature appFeature);
        Task RestrictAccessAsync(AccessGroup accessGroup, AppFeature feature);
        Task RestrictAccessAsync(string roleName, int appFeatureId);
        Task SavePermissionsAsync(List<Role> roles, List<AppFeatureItem> appFeatures, IFormCollection form);
    }
}