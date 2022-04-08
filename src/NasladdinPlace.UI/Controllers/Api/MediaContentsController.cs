using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(PosCrudPermission))]
    public class MediaContentsController : BaseController
    {
        private static readonly string[] AllowedContentTypes = { "image/jpg", "image/jpeg", "image/png", "image/gif" };
        private static readonly string[] AllowedFileExtension = { ".jpg", ".jpeg", ".png", ".gif" };

        public MediaContentsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> UploadImagesAsync()
        {
            var files = HttpContext.Request.Form.Files;
            if (!files.All(IsValidFile))
                return BadRequest(new ErrorResponseDto { Error = "Загружаемый файл не имеет нужное расширение" });

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);

                    var mediaContentFile = new MediaContent(
                        contentType: MediaContentType.Image,
                        fileName: file.FileName,
                        fileContent: ms.ToArray());

                    UnitOfWork.GetRepository<MediaContent>().Add(mediaContentFile);
                }
            }

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaContentAsync(int id)
        {
            var mediaContentRepository = UnitOfWork.GetRepository<MediaContent>();
            var mediaContent = mediaContentRepository.GetById(id);

            if (mediaContent == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о записи медиа контента не найдена" });

            mediaContentRepository.Remove(id);
            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        private bool IsValidFile(IFormFile file)
        {
            if (!AllowedContentTypes.Contains(file.ContentType.ToLower()))
                return false;

            if (!AllowedFileExtension.Contains(Path.GetExtension(file.FileName).ToLower()))
                return false;

            return true;
        }
    }
}