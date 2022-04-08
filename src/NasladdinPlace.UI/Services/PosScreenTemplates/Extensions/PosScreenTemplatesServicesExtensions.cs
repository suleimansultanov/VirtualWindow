using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.UI.Managers.PosScreenTemplates;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Models;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Validators;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Validators.Contracts;

namespace NasladdinPlace.UI.Services.PosScreenTemplates.Extensions
{
    public static class PosScreenTemplatesServicesExtensions
    {
        public static void AddPosScreenTemplates(this IServiceCollection services,
            IConfigurationReader configurationReader)
        {
	        var defaultPosScreenTemplateId = configurationReader.GetDefaultPosScreenTemplateId();

            services.AddTransient<IPosScreenTemplateValidator>(sp => new PosScreenTemplateValidator(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                defaultPosScreenTemplateId
            ));

            var filesCommonDirectoryName = configurationReader.GetFilesCommonDirectoryName();
            var templateDirectoryNameFormat = configurationReader.GetTemplateDirectoryNameFormat();
            var requiredFilesList = configurationReader.GetRequiredFilesList();

            var webRootPath = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>().WebRootPath;

            var posScreenTemplatesFilesInfoProvider =
                new PosScreenTemplatesFilesInfo(
                    webRootPath,
                    filesCommonDirectoryName,
                    templateDirectoryNameFormat,
                    requiredFilesList);

            services.AddTransient<IPosScreenTemplateFilesManager>(sp => new PosScreenTemplateFilesManager(posScreenTemplatesFilesInfoProvider));
            services.AddTransient<IPosScreenTemplateManager>(sp => new PosScreenTemplateManager(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                sp.GetRequiredService<IPosScreenTemplateFilesManager>(),
                sp.GetRequiredService<IPosScreenTemplateValidator>(),
                defaultPosScreenTemplateId));
        }
    }
}