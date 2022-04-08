using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Models
{
    public class RolePermittedAppFeature
    {   
        public int RoleId { get; private set; }
        public AppFeature AppFeature { get; private set; }

        public RolePermittedAppFeature()
        {
            // required for EF
        }

        public RolePermittedAppFeature(int roleId, AppFeature appFeature)
        {
            RoleId = roleId;
            AppFeature = appFeature;
        }
    }
}