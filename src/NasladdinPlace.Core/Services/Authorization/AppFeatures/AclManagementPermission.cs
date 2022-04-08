using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Services.Authorization.AppFeatures
{
    public class AclManagementPermission : BasePermission<AclManagementPermission>, IPermissionInitializer
    {
        public PermissionCategory InitializePermissionCategory => PermissionCategory.Administration;
    }
}
