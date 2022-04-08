using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Media;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    [Authorize]
    [Permission(nameof(PosCrudPermission))]
    [Route(Routes.Api)]
    public class MediaContentToPlatformsController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public MediaContentToPlatformsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpPost]
        public async Task<IActionResult> AddMediaContentToPlatformAsync(
            [FromBody] MediaContentToPosPlatformViewModel platformViewModel)
        {
            var mediaContent = UnitOfWork.GetRepository<MediaContent>().GetById(platformViewModel.MediaContentId);
            if (mediaContent == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте не найдена" });

            var mediaContentToPlatform = new MediaContentToPosPlatform(
                mediaContentId: mediaContent.Id,
                posScreenType: (PosScreenType)platformViewModel.PosScreenType);

            UnitOfWork.GetRepository<MediaContentToPosPlatform>().Add(mediaContentToPlatform);

            await UnitOfWork.CompleteAsync();

            await _nasladdinApiClient.RefreshAllDisplayPageAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditMediaContentToPlatformAsync(
            [FromBody] MediaContentToPosPlatformViewModel platformViewModel)
        {
            var mediaContent = UnitOfWork.GetRepository<MediaContent>().GetById(platformViewModel.MediaContentId);
            if (mediaContent == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте не найдена" });

            var mediaContentToPlatform = UnitOfWork.GetRepository<MediaContentToPosPlatform>()
                .GetById(platformViewModel.MediaContentToPosPlatformId);
            if (mediaContentToPlatform == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о привязке медиа контента не найдена" });

            mediaContentToPlatform.Update(
                mediaContentId: mediaContent.Id,
                posScreenType: (PosScreenType)platformViewModel.PosScreenType);

            await UnitOfWork.CompleteAsync();

            await _nasladdinApiClient.RefreshAllDisplayPageAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaContentToPlatformAsync(int id)
        {
            var mediaContentToPlatformRepository = UnitOfWork.GetRepository<MediaContentToPosPlatform>();
            var mediaContent = mediaContentToPlatformRepository.GetById(id);

            if (mediaContent == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте не найдена" });

            mediaContentToPlatformRepository.Remove(id);
            await UnitOfWork.CompleteAsync();

            return Ok();
        }
    }
}