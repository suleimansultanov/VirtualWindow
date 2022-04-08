using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Authorization;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Providers;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.DAL.Repositories;
using System;
using System.Linq;

namespace NasladdinPlace.UI.Extensions
{
    public static class PermissionsExtensions
    {
        public static void AddAppFeatures(this IServiceCollection services)
        {
            services.AddTransient<IAppFeatureItemsRepository, AppFeatureItemsRepository>();
            services.AddTransient<IAccessGroupAppFeaturesAccessManager, AccessGroupAppFeaturesAccessManager>();
        }

        public static void UseAppFeatures(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var interfaceType = typeof(IPermissionInitializer);
                var permissionTypesInitializers = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass);

                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var role = unitOfWork.Roles.GetByNameIncludingPermittedAppFeaturesAsync(Roles.Admin).GetAwaiter().GetResult();

                foreach (var permissionTypeInitializer in permissionTypesInitializers)
                {
                    var appFeatureInitializer = (IPermissionInitializer)Activator.CreateInstance(permissionTypeInitializer);
                    var permission = scope.ServiceProvider.GetRequiredService<IAppFeatureItemsRepository>()
                        .GetPermissionForInitialize(appFeatureInitializer, role.Id);

                    if (permission != null)
                        unitOfWork.AppFeatureItems.Add(permission);
                }

                unitOfWork.CompleteAsync().GetAwaiter().GetResult();
            }
        }

        public static void AssignAppFeaturesToRoles(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var availableAppfeaturesAndRoles = AppFeaturesToRole.GetAvailableAppFeaturesForRoles();

                foreach (var appfeaturesAndRole in availableAppfeaturesAndRoles)
                {
                    var currentRole = unitOfWork.Roles.GetByNameIncludingPermittedAppFeaturesAsync(appfeaturesAndRole.Key)
                        .GetAwaiter().GetResult();

                    currentRole.InternalPermittedFeatures.Clear();

                    foreach (var appFeatureType in appfeaturesAndRole.Value)
                    {
                        var appFeature = unitOfWork.AppFeatureItems.GetByName(appFeatureType.Name);
                        if (appFeature != null)
                            currentRole.InternalPermittedFeatures.Add(new AppFeatureItemsToRole(currentRole.Id, appFeature.Id));
                    }
                }

                unitOfWork.CompleteAsync().GetAwaiter().GetResult();
            }
        }
    }
}
