using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.DAL.Repositories
{
    public class AppFeatureItemsRepository : Repository<AppFeatureItem>, IAppFeatureItemsRepository
    {
        public AppFeatureItemsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<AppFeatureItem>> GetAllIncludingRolePermittedFeaturesAsync()
        {
            return GetAll()
                .Include(pr => pr.RolePermittedFeatures)
                .ToListAsync();
        }

        public AppFeatureItem GetByName(string appFeatureName)
        {
            return GetAll()
                .Include(p => p.RolePermittedFeatures)
                .SingleOrDefault(pr => pr.Name == appFeatureName);
        }

        public AppFeatureItem GetPermissionForInitialize(IPermissionInitializer permissionInitializer, int roleId)
        {
            if (permissionInitializer == null)
                throw new ArgumentNullException(nameof(permissionInitializer));

            var existingAppFeature = GetByName(permissionInitializer.InitializeName);

            if (existingAppFeature != null)
                return null;

            var appFeature = AppFeatureItem.CreateAppFeatureItem();

            appFeature.InitializeAppFeatureItem(permissionInitializer, roleId);
            
            return appFeature;
        }

        public void Remove(AppFeatureItem appFeatureItem)
        {
            appFeatureItem.RolePermittedFeatures.Clear();
            Context.AppFeatureItems.Remove(appFeatureItem);
        }
    }
}
