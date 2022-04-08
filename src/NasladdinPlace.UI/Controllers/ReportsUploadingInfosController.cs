using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Reports;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class ReportsUploadingInfosController : ReferenceBaseController
    {
        public ReportsUploadingInfosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetReportsUploadingInfos()
        {
            return Reference<ReportUploadingInfoViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Ежедневные отчеты",
                    BeforeGridPartialName = "~/Views/ReportsUploadingInfos/_beforeGridPartial.cshtml",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.add"
                        },
                    }
                });
        }
    }
}