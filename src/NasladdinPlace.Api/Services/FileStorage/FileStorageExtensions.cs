using Microsoft.Extensions.DependencyInjection;

namespace NasladdinPlace.Api.Services.FileStorage
{
    public static class FileStorageExtensions
    {
        public static void AddFileSystemStorage(this IServiceCollection services)
        {
            services.AddScoped<IFileStorage, FileSystemStorage>();
        }
    }
}
