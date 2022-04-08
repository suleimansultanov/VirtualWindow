using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.FileStorage;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Logging;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/goods/{goodId}/images")]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class GoodImagesController : Controller
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        private readonly string _destinationDirectory;

        public GoodImagesController(IServiceProvider serviceProvider)
        {
            _fileStorage = serviceProvider.GetRequiredService<IFileStorage>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _destinationDirectory = serviceProvider.GetRequiredService<IConfigurationReader>().GetGoodImageDirectory();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [HttpPost]
        public async Task<IActionResult> AddGoodImageAsync(int goodId, IFormFile goodImageFile, int userId)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var good = await unitOfWork.Goods.GetIncludingImagesAsync(goodId);

                    if (good != null && !string.IsNullOrEmpty(good.GetGoodImagePath()))
                    {
                        await _fileStorage.DeleteFile(good.GetGoodImagePath());
                        _logger.LogInfo($"User {userId} removed an image {good.GetGoodImagePath()}");
                    }

                    var saveFileResult = await _fileStorage.SaveFile(goodImageFile, _destinationDirectory);

                    if (!saveFileResult.Succeeded)
                        BadRequest("Some error has occured during saving an image.");

                    _logger.LogInfo($"User {userId} added an image {saveFileResult.FileRelativePath}");

                    return Ok(saveFileResult.FileRelativePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occurred during saving an image. Verbose error: {ex}");
                return BadRequest(
                    "An error has occurred during saving an image.");
            }
        }
    }
}
