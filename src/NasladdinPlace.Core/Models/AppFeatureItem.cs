using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Models
{
    public class AppFeatureItem : Entity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public PermissionCategory PermissionCategory { get; private set; }

        public ICollection<AppFeatureItemsToRole> RolePermittedFeatures { get; private set; }

        public static AppFeatureItem CreateAppFeatureItem()
        {
            return new AppFeatureItem();
        }

        protected AppFeatureItem()
        {
            RolePermittedFeatures = new Collection<AppFeatureItemsToRole>();
        }

        public void InitializeAppFeatureItem(IPermissionInitializer permissionInitializer, int roleId)
        {
            if (permissionInitializer == null)
                throw new ArgumentNullException(nameof(permissionInitializer));

            PermissionCategory = permissionInitializer.InitializePermissionCategory;
            Description = permissionInitializer.InitializeDescription;
            Name = permissionInitializer.InitializeName;
            RolePermittedFeatures.Add(new AppFeatureItemsToRole(roleId, Id));
        }
    }
}
