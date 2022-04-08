using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IAppFeatureItemsRepository : IRepository<AppFeatureItem>
    {
        Task<List<AppFeatureItem>> GetAllIncludingRolePermittedFeaturesAsync();
        AppFeatureItem GetByName(string appFeatureName);
        AppFeatureItem GetPermissionForInitialize(IPermissionInitializer permissionInitializer, int roleId);
        void Remove(AppFeatureItem appFeatureItem);
    }
}
