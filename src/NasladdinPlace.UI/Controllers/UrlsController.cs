using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Services.Configuration.Reader;
using System;

namespace NasladdinPlace.UI.Controllers
{
    public class UrlsController : Controller
    {
        private readonly IConfigurationReader _configurationReader;

        public UrlsController(IConfigurationReader configurationReader) 
        {
            if(configurationReader == null)
                throw new ArgumentNullException(nameof(configurationReader));

            _configurationReader = configurationReader;
        }

        [HttpGet]
        public IActionResult GenerateUrlsJsFile()
        {
            var baseApiUrl = _configurationReader.GetBaseApiUrl();
            var baseAdminUrl = _configurationReader.GetAdminPageBaseUrl();
            var baseApiWsUrl = ConfigurationReaderExt.GetWsUrl(baseApiUrl);
            var postfixUrl = _configurationReader.GetPlantControllerPostfixUrl();
            var wsPlantUrl = ConfigurationReaderExt.CombineUrlParts(baseApiWsUrl, postfixUrl);
            var script = $"var ApiUrl = '{baseApiUrl}'; var WsUrl = '{wsPlantUrl}'; var AdminUrl = '{baseAdminUrl}';";

            return Content(script);
        }
    }
}