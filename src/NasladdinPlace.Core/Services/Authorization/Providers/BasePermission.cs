using NasladdinPlace.Core.Services.Authorization.AppFeatures.Resources;

namespace NasladdinPlace.Core.Services.Authorization.Providers
{
    public abstract class BasePermission<TPermission>
    {
        public string InitializeName => typeof(TPermission).Name;
        public string InitializeDescription => PermissionsResource.ResourceManager.GetString(typeof(TPermission).Name);
    }
}
