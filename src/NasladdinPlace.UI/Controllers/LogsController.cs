using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Logs;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class LogsController : ReferenceBaseController
    {
        public LogsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Route("Logs")]
        public IActionResult GetLogs(int id)
        {
            ViewData["PosId"] = id;
            return View();
        }

        public IActionResult GetPosLogs()
        {
            return Reference<PosLogViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Логи витрин",
                    BeforeGridPartialName = "~/Views/Logs/_beforeGridPartial.cshtml",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilterFromToDateTime(filter, topFilter, nameof(PosLogViewModel.DateTimeCreated), null, null, "Дата получения c", "Дата получения по");
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(PosLogViewModel.DateTimeCreated), SortTypes.Desc);
                    },
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Запросить",
                            Binding = "click: $root.requestLogs"
                        }
                    },
                }
            );

        }
    }
}
