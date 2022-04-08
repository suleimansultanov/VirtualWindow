using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.PosScreenTemplates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class PosScreenTemplatesController : ReferenceBaseController
    {
        private readonly IPosScreenTemplateFilesManager _posScreenTemplateFilesManager;

        public PosScreenTemplatesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _posScreenTemplateFilesManager = serviceProvider.GetRequiredService<IPosScreenTemplateFilesManager>();
        }

        public IActionResult GetTemplates()
        {
            return Reference<PosScreenTemplateViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Шаблоны",
                    BeforeGridPartialName = "~/Views/PosScreenTemplates/_beforePosScreenTemplatesGridPartial.cshtml",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.add"
                        }
                    }
                });
        }

        [Permission(nameof(PosScreenTemplateEditPermission))]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> EditTemplateAsync(int id)
        {
            var template = await UnitOfWork.PosScreenTemplates.GetIncludingPointsOfSaleAsync(id);
            if (template == null)
                return BadRequest(new ErrorResponseDto { Error = $"Шаблон с идентификатором {id} не существует" });

            var viewModel = Mapper.Map<PosScreenTemplateFormViewModel>(template);

            viewModel.TemplateFiles = _posScreenTemplateFilesManager.GetTemplateFilesNames(template.Id);
            viewModel.ScreenResolutions = ScreenResolutionsProvider.GetValues();

            return View("PosScreenTemplateForm", viewModel);
        }
    }
}