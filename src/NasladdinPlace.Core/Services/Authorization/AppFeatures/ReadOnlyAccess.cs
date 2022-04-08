using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Services.Authorization.AppFeatures
{
    public class ReadOnlyAccess : BasePermission<ReadOnlyAccess>, IPermissionInitializer
    {
        public PermissionCategory InitializePermissionCategory => PermissionCategory.Common;
    }
}
