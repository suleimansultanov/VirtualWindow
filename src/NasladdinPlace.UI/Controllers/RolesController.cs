using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Roles;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(AclManagementPermission))]
    public class RolesController : ReferenceBaseController
    {
        public RolesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetRoles()
        {
            return Reference<RoleViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Список ролей",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.openAddModal",
                            ClassStyle = "btn btn-primary add-role"
                        }
                    },
                    BeforeGridPartialName = "~/Views/Roles/_beforeRolesGridPartial.cshtml",
                });
        }
    }
}