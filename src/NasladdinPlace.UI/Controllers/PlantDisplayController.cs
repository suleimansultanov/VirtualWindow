using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers
{
    [AllowAnonymous]
    [EnableCors(CorsPolicies.AllowFullAccess)]
    public class PlantDisplayController : BaseController
    {
        private readonly IPosScreenTemplateFilesManager _posScreenTemplateFilesManager;

        public PlantDisplayController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _posScreenTemplateFilesManager = serviceProvider.GetRequiredService<IPosScreenTemplateFilesManager>();
        }

        [Route("[controller]/Index/{posId}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> IndexAsync(int posId)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdIncludingPosScreenTemplateAsync(posId);

            if (pos == null)
                return BadRequest(new ErrorResponseDto { Error = $"Витрина с идентификатором {posId} не существует" });

            if (pos.PosScreenTemplate == null)
                return BadRequest(new ErrorResponseDto
                { Error = $"Шаблон с идентификатором {pos.PosScreenTemplateId} не существует" });

            var requiredTemplateFile =
                _posScreenTemplateFilesManager.GetMissingRequiredFileNamesForTemplate(pos.PosScreenTemplateId);

            if (requiredTemplateFile.Any())
                return BadRequest(new ErrorResponseDto
                { Error = $"Шаблон с идентификатором {pos.PosScreenTemplateId} не содержит файлов" });

            var redirectUrl =
                $"{_posScreenTemplateFilesManager.GetTemplateDirectoryPath(pos.PosScreenTemplateId)}/index.html?posId={posId}";

            return Redirect($"~/{redirectUrl}");
        }
    }
}