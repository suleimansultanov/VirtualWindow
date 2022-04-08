using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(PosCrudPermission))]
    public class PlantImagesController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public PlantImagesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlantImageAsync(int id)
        {
            var deletePosImageResult = await _nasladdinApiClient.DeletePosImageAsync(id);

            return deletePosImageResult.ToActionResult();
        }
    }
}