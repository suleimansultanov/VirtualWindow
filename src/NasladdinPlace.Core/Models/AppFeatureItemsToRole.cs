using System;

namespace NasladdinPlace.Core.Models
{
    public class AppFeatureItemsToRole
    {
        public int RoleId { get; private set; }
        public int AppFeatureItemId { get; private set; }

        public AppFeatureItemsToRole()
        {
            // required for EF
        }

        public AppFeatureItemsToRole(int roleId, int appFeatureItemId)
        {
            if (roleId < 0)
                throw new ArgumentOutOfRangeException(nameof(roleId), roleId, "RoleId must be grater than zero.");
            if (appFeatureItemId < 0)
                throw new ArgumentOutOfRangeException(nameof(appFeatureItemId), appFeatureItemId, "AppFeatureItemId must be grater than zero.");

            RoleId = roleId;
            AppFeatureItemId = appFeatureItemId;
        }
    }
}
