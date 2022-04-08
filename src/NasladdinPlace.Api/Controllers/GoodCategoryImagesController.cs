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
    [Route("api/goodCategories/{goodCategoryId}/image")]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class GoodCategoryImagesController : Controller
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        private readonly string _destinationDirectory;

        public GoodCategoryImagesController(IServiceProvider serviceProvider)
        {
            _fileStorage = serviceProvider.GetRequiredService<IFileStorage>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _destinationDirectory = serviceProvider.GetRequiredService<IConfigurationReader>().GetGoodCategoryImageDirectory();
        }

        [HttpPost]
        public async Task<IActionResult> AddGoodCategoryImageAsync(int goodCategoryId, IFormFile goodCategoryImageFile, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var goodCategory = await unitOfWork.GoodCategories.GetAsync(goodCategoryId);

                    if (goodCategory != null && !string.IsNullOrEmpty(goodCategory.ImagePath))
                    {
                        await _fileStorage.DeleteFile(goodCategory.ImagePath);
                        _logger.LogInfo($"User {userId} removed an image {goodCategory.ImagePath}");
                    }

                    var saveFileResult = await _fileStorage.SaveFile(goodCategoryImageFile, _destinationDirectory);

                    if (!saveFileResult.Succeeded)
                        return BadRequest("An error has occurred during saving an image.");

                    _logger.LogInfo($"User {userId} added an image {saveFileResult.FileRelativePath}");

                    return Ok(saveFileResult.FileRelativePath);
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
}