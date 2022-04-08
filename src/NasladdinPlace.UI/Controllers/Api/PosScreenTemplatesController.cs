using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.PosScreenTemplates;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.ViewModels.PosScreenTemplates;
using NasladdinPlace.Utilities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(PosScreenTemplateEditPermission))]
    public class PosScreenTemplatesController : BaseController
    {
        private readonly IPosScreenTemplateManager _posScreenTemplateManager;
        private readonly IPosScreenTemplateFilesManager _posScreenTemplateFilesManager;
        private readonly IContentTypeProvider _fileProvider;

        public PosScreenTemplatesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _posScreenTemplateManager = serviceProvider.GetRequiredService<IPosScreenTemplateManager>();
            _posScreenTemplateFilesManager = serviceProvider.GetRequiredService<IPosScreenTemplateFilesManager>();
            _fileProvider = serviceProvider.GetRequiredService<IContentTypeProvider>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplateAsync(
            [FromBody] CreatePosScreenTemplateViewModel templateCreatingViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var templateCreationResult =
                await _posScreenTemplateManager.CreatePosScreenTemplateAsync(templateCreatingViewModel.Name);

            return ReturnOkOrBadRequestUsingResult(templateCreationResult);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UploadFileAsync(int id)
        {
            var files = HttpContext.Request.Form.Files;

            if (!files.Any())
                return BadRequest(new ErrorResponseDto { Error = "Запрос не содержит файлов для сохранения" });

            var templateFileUploadingResult =
                await _posScreenTemplateManager.UploadTemplateFileAsync(id, files.First());

            return ReturnOkOrBadRequestUsingResult(templateFileUploadingResult);
        }

        [HttpPut]
        public async Task<IActionResult> EditTemplateAsync(
            [FromBody] EditPosScreenTemplateViewModel templateEditingViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var templateEditingResult =
                await _posScreenTemplateManager.EditPosScreenTemplateAsync(
                    templateEditingViewModel.Id,
                    templateEditingViewModel.Name,
                    templateEditingViewModel.PointsOfSale);

            return ReturnOkOrBadRequestUsingResult(templateEditingResult);
        }

        [HttpDelete("{id}/{fileName}")]
        public async Task<IActionResult> DeleteFileAsync(DeletePosScreenTemplateFileViewModel deleteFileViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var templateFileDeletionResult =
                await _posScreenTemplateManager.DeleteTemplateFileAsync(deleteFileViewModel.Id, deleteFileViewModel.FileName);

            return ReturnOkOrBadRequestUsingResult(templateFileDeletionResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplateAsync(int id)
        {
            var templateDeletionResult = await _posScreenTemplateManager.DeletePosScreenTemplateAsync(id);

            return ReturnOkOrBadRequestUsingResult(templateDeletionResult);
        }

        [HttpGet("{id}/{fileName}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetTemplateFile(int id, string fileName)
        {
            var template = await UnitOfWork.PosScreenTemplates.GetIncludingPointsOfSaleAsync(id);

            if (template == null)
                return BadRequest(new ErrorResponseDto { Error = $"Шаблон с идентификатором {id} не существует" });

            var filePath = $"{_posScreenTemplateFilesManager.GetTemplateDirectoryPath(id)}/{fileName}";
            var contentType = _fileProvider.TryGetContentType(fileName, out var type) ? type : ContentTypes.TextHtml;

            return File(filePath, contentType);
        }

        private IActionResult ReturnOkOrBadRequestUsingResult(Result result)
        {
            return !result.Succeeded ? (IActionResult)BadRequest(result.Error) : Ok();
        }
    }
}