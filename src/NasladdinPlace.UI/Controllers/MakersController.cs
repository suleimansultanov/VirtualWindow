using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Makers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class MakersController : ReferenceBaseController
    {
        public MakersController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetMakers()
        {
            return Reference<MakerViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Список производителей",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.add",
                            ClassStyle = "btn btn-primary add-maker"
                        }
                    },
                    BeforeGridPartialName = "~/Views/Makers/_beforeMakersGridPartial.cshtml",
                });
        }
    }
}