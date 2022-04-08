using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NasladdinPlace.UI.Resources;
using System.Reflection;

namespace NasladdinPlace.UI.Services.Localization
{
    public class MvcLocalizationConfiguration : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizer _stringLocalizer;

        public MvcLocalizationConfiguration()
        {
        }

        public MvcLocalizationConfiguration(IStringLocalizerFactory stringLocalizerFactory)
        {
            var assemblyName = new AssemblyName(typeof(ModelBindingMessagesLocalization).GetTypeInfo().Assembly.FullName);
            _stringLocalizer = stringLocalizerFactory.Create("ModelBindingMessagesLocalization", assemblyName.Name);
        }

        public void Configure(MvcOptions options)
        {
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) =>
                _stringLocalizer["The value '{0}' is invalid."]);

            options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) =>
                _stringLocalizer["The field {0} must be a number.", x]);
        }
    }
}
