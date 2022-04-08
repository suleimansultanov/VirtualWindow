using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Services.Authorization.AppFeatures
{
    public class LabeledGoodManagementPermission : BasePermission<LabeledGoodManagementPermission>, IPermissionInitializer
    {
        public PermissionCategory InitializePermissionCategory => PermissionCategory.Sales;
    }
}
