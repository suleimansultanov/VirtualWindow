using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NasladdinPlace.Application.Services.PosOperations.Factory;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class PosOperationsAppServiceExtensions
    {
        public static void AddPosOperationsAppService(this IServiceCollection services)
        {
            services.AddActionExecutionUtilities();
            services.TryAddScoped(PosOperationsAppServicesFactory.Create);
        }
    }
}