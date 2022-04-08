using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class PlantImagesController : Controller
    {
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly string _baseApiUrl;

        public PlantImagesController(INasladdinApiClient nasladdinApiClient,
            IConfigurationReader configurationReader)
        {
            if(configurationReader==null)
                throw new ArgumentNullException(nameof(configurationReader));
            _nasladdinApiClient = nasladdinApiClient;
            _baseApiUrl = configurationReader.GetBaseApiUrl();
        }

        public async Task<IActionResult> AddPlantImageAsync(int resourceId)
        {
            var goodResult = await _nasladdinApiClient.GetPosAsync(resourceId);

            if (goodResult.Status != ResultStatus.Success)
                return RedirectToImagesList(resourceId);

            var viewModel = new ImageFormViewModel
            {
                ResourceName = goodResult.Result.Name,
                ResourceId = goodResult.Result.Id,
                AspController = nameof(PlantImagesController),
                AspAction = nameof(AddPlantImageAsync)
            };

            return View("ImageForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlantImageAsync(ImageFormViewModel viewModel)
        {
            await _nasladdinApiClient.AddPosImageAsync(viewModel.ResourceId, viewModel.ImageFile);
            return RedirectToImagesList(viewModel.ResourceId);
        }

        public async Task<IActionResult> GetPlantImagesAsync(int plantId)
        {
            var goodImagesResult = await _nasladdinApiClient.GetPosImagesAsync(plantId);
            var posResult = await _nasladdinApiClient.GetPosAsync(plantId);

            if (goodImagesResult.Status != ResultStatus.Success || posResult.Status != ResultStatus.Success)
                return this.RedirectToHome();

            var images = goodImagesResult.Result.ToList();

            foreach (var image in images)
            {
                image.ImagePath = _baseApiUrl + "/" + image.ImagePath;
            }

            var viewModel = new ImagesViewModel(
                images,
                posResult.Result,
                "js-delete-plantImage",
                nameof(PlantImagesController),
                nameof(AddPlantImageAsync));

            return View("GetImages", viewModel);
        }

        private IActionResult RedirectToImagesList(int plantId)
        {
            return RedirectToAction("GetPlantImagesAsync", new { plantId });
        }
    }
}