using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Authorization.Providers
{
    public interface IPermissionInitializer
    {
        string InitializeName { get; }
        string InitializeDescription { get; }
        PermissionCategory InitializePermissionCategory { get; }
    }
}
