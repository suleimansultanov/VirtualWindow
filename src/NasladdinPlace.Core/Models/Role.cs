using System;
using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Services.Authorization.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NasladdinPlace.Core.Models
{
    public class Role : IdentityRole<int>
    {
        public static Role FromName(string roleName)
        {
            if(string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            return new Role
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
        }

        public static Role FromNameAndDescription(string roleName, string description)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            return new Role
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                Description = description
            };
        }

        public ICollection<RolePermittedAppFeature> InternalPermittedAppFeatures { get; private set; }
        public ICollection<AppFeatureItemsToRole> InternalPermittedFeatures { get; private set; }
        public ICollection<PointsOfSaleToRole> InternalAssignedPoses { get; private set; }
        public ICollection<UserRole> UserRoles { get; private set; }

        public string Description { get; set; }

        protected Role()
        {
            InternalPermittedAppFeatures = new Collection<RolePermittedAppFeature>();
            InternalPermittedFeatures = new Collection<AppFeatureItemsToRole>();
            InternalAssignedPoses = new Collection<PointsOfSaleToRole>();
            UserRoles = new Collection<UserRole>();
        }

        public ICollection<AppFeature> PermittedAppFeatures =>
            InternalPermittedAppFeatures.Select(iaf => iaf.AppFeature).ToHashSet();

        public ICollection<int> PermittedFeatures =>
            InternalPermittedFeatures.Select(iaf => iaf.AppFeatureItemId).ToHashSet();

        public ICollection<int> AssignedPoses =>
            InternalAssignedPoses.Select(iap => iap.PosId).ToHashSet();

        public void PermitAppFeature(AppFeature appFeature)
        {
            if (InternalPermittedAppFeatures.Any(ipaf => ipaf.AppFeature == appFeature)) return;

            InternalPermittedAppFeatures.Add(new RolePermittedAppFeature(0, appFeature));
        }

        public void PermitAppFeature(int roleId, int appFeatureRecordId)
        {
            if (InternalPermittedFeatures.Any(ipaf => ipaf.AppFeatureItemId == appFeatureRecordId)) return;

            InternalPermittedFeatures.Add(new AppFeatureItemsToRole(roleId, appFeatureRecordId));
        }

        public void RestrictAppFeature(AppFeature appFeature)
        {
            var permittedAppFeatureToRestrict = InternalPermittedAppFeatures.SingleOrDefault(
                paf => paf.AppFeature == appFeature
            );
            InternalPermittedAppFeatures.Remove(permittedAppFeatureToRestrict);
        }

        public void RestrictAppFeature(int appFeatureRecordId)
        {
            var permittedAppFeatureToRestrict = InternalPermittedFeatures.SingleOrDefault(
                paf => paf.AppFeatureItemId == appFeatureRecordId
            );
            InternalPermittedFeatures.Remove(permittedAppFeatureToRestrict);
        }

        public bool IsAppFeaturePermitted(AppFeature appFeature)
        {
            return PermittedAppFeatures.Contains(appFeature);
        }

        public bool IsAppFeaturePermitted(int appFeatureRecordId)
        {
            return PermittedFeatures.Contains(appFeatureRecordId);
        }
    }
}