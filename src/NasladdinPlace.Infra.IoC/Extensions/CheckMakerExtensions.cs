using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class CheckMakerExtensions
    {
        public static void AddUsersUnpaidOperationsChecksMaker(this IServiceCollection services)
        {
            services.AddTransient<IUsersUnpaidOperationsChecksMaker, UsersUnpaidOperationsChecksMaker>();
        }
    }
}
