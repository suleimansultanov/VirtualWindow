using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    [Route(Routes.Api)]
    public class PosMediaContentsController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public PosMediaContentsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [AllowAnonymous]
        [HttpGet("/api/posMediaContents/{posId}/{screenType}")]
        public async Task<IActionResult> GetPosImageAsync(int posId, PosScreenType screenType)
        {
            var posMediaContent =
                await UnitOfWork.PosMediaContents.GetByPosIdAndScreenImageTypeAsync(posId, screenType);
            if (posMediaContent != null)
                return GetMediaContentFileResult(posMediaContent.MediaContent);

            var platformMediaContent =
                await UnitOfWork.MediaContentToPosPlatforms.GetLastMediaContentToPosPlatformByScreenType(screenType);

            return platformMediaContent != null
                ? GetMediaContentFileResult(platformMediaContent.MediaContent)
                : NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте для платформы не найдена" });
        }

        private IActionResult GetMediaContentFileResult(MediaContent mediaContent)
        {
            var fileExtension = Path.GetExtension(mediaContent.FileName).ToLower();
            var contentType = $"image/{fileExtension.Replace(".", "")}";
            var fileName = $@"{Guid.NewGuid():N}{fileExtension}";

            return File(mediaContent.FileContent, contentType, fileName);
        }

        [HttpPost]
        [Authorize]
        [Permission(nameof(PosCrudPermission))]
        public async Task<IActionResult> AddMediaContentToPosAsync([FromBody] PosMediaContentViewModel viewModel)
        {
            var mediaContent = UnitOfWork.GetRepository<MediaContent>().GetById(viewModel.MediaContentId);
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(viewModel.PosId);

            if (mediaContent == null || pos == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте не найдена" });

            var existingPosMediaContent =
                await UnitOfWork.PosMediaContents.GetByPosIdMediaContentIdAsync(pos.Id, mediaContent.Id);

            if (existingPosMediaContent != null)
                return BadRequest(new ErrorResponseDto
                { Error = "Информация о выбранном медиа контенте уже закреплена за витриной" });

            var posMediaContent = new PosMediaContent(
                posId: pos.Id,
                mediaContentId: mediaContent.Id,
                posScreenType: (PosScreenType)viewModel.PosScreenType);

            UnitOfWork.PosMediaContents.Add(posMediaContent);
            await UnitOfWork.CompleteAsync();

            await _nasladdinApiClient.RefreshDisplayPageAsync(pos.Id);

            return Ok();
        }

        [Authorize]
        [Permission(nameof(PosCrudPermission))]
        [HttpDelete("/api/posMediaContents/{posId}/{mediaContentId}")]
        public async Task<IActionResult> DeletePosMediaContentAsync(int posId, int mediaContentId)
        {
            var posMediaContent =
                await UnitOfWork.PosMediaContents.GetByPosIdMediaContentIdAsync(posId, mediaContentId);

            if (posMediaContent == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о медиа контенте для витрины не найдена" });

            UnitOfWork.PosMediaContents.Remove(posMediaContent);
            await UnitOfWork.CompleteAsync();

            return Ok();
        }
    }
}