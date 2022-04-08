using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization;
using NasladdinPlace.Core.Services.Authorization.Contracts;

namespace NasladdinPlace.UI.Services.Authorization
{
    public static class UserAppFeaturesAuthorizationExtensions
    {
        public static void AddUserAppFeaturesAuthorization(this IServiceCollection services)
        {
            services.AddTransient<IUserAccessGroupMembershipManager, UserAccessGroupMembershipManager>();
            services.AddTransient<IAccessGroupAppFeaturesAccessManager, AccessGroupAppFeaturesAccessManager>();
            services.AddSingleton(sp =>
            {
                var userAppFeaturesAccessChecker = new UserAppFeaturesAccessChecker(
                    sp.GetRequiredService<IAccessGroupAppFeaturesAccessManager>(),
                    sp.GetRequiredService<IUserAccessGroupMembershipManager>()
                );
                SharedUserAppFeaturesAccessChecker.Initialize(userAppFeaturesAccessChecker);
                return SharedUserAppFeaturesAccessChecker.Instance;
            });
        }
    }
}