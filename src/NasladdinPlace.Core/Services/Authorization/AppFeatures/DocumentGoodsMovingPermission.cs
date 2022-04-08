using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Authorization.Providers;

namespace NasladdinPlace.Core.Services.Authorization.AppFeatures
{
    public class DocumentGoodsMovingPermission : BasePermission<DocumentGoodsMovingPermission>, IPermissionInitializer
    {
        public PermissionCategory InitializePermissionCategory => PermissionCategory.Sales;
    }
}
