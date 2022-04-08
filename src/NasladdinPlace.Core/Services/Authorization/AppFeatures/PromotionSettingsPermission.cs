using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Services.Authorization.AppFeatures
{
    public class PromotionSettingsPermission : BasePermission<PromotionSettingsPermission>, IPermissionInitializer
    {
        public PermissionCategory InitializePermissionCategory => PermissionCategory.Marketing;
    }
}
