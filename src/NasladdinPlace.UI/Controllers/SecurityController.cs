using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Dtos.Permission;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using Microsoft.AspNetCore.Http;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.ViewModels.Roles;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(AclManagementPermission))]
    public class SecurityController : ReferenceBaseController
    {
        private readonly IAppFeatureItemsRepository _appFeatureItemsRepository;
        private readonly IAccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;

        public SecurityController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _accessGroupAppFeaturesAccessManager =
                serviceProvider.GetRequiredService<IAccessGroupAppFeaturesAccessManager>();
            _appFeatureItemsRepository = serviceProvider.GetRequiredService<IAppFeatureItemsRepository>();
        }

        public IActionResult GetPermissions()
        {
            return Reference<AppFeaturesViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Список разрешений",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.OpenUrl,
                            Name = "<em class='fa fa-arrows-alt'></em> Управление разрешениями",
                            Url = Url.Action("Edit", "Security"),
                            ClassStyle = "btn btn-primary edit-app-features"
                        }
                    }
                });
        }

        public async Task<IActionResult> Edit()
        {
            var model = await PreparePermissionMappingModelAsync(new EditAppFeaturesViewModel());

            return View("EditPermissionForRoleForm", model);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> PermissionsSave(IFormCollection form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roles = await UnitOfWork.Roles.GetAllAsync();
            var permissionRecords = await _appFeatureItemsRepository.GetAllIncludingRolePermittedFeaturesAsync();

            await _accessGroupAppFeaturesAccessManager.SavePermissionsAsync(roles, permissionRecords, form);

            return RedirectToAction("Edit");
        }

        private Task<EditAppFeaturesViewModel> PreparePermissionMappingModelAsync(EditAppFeaturesViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return PreparePermissionMappingModelAuxAsync(model);
        }

        private async Task<EditAppFeaturesViewModel> PreparePermissionMappingModelAuxAsync(EditAppFeaturesViewModel model)
        {
            var customerRoles = await UnitOfWork.Roles.GetAllAsync();
            model.AvailableRoles = customerRoles.Select(role => new RoleViewModel { Name = role.Name, Id = role.Id }).ToList();
            var permissionRecords = await _appFeatureItemsRepository.GetAllIncludingRolePermittedFeaturesAsync();

            foreach (var permissionRecord in permissionRecords)
            {
                model.AvailableAppFeatures.Add(new AppFeatureItemDto
                {
                    Name = permissionRecord.Name,
                    PermissionCategory = permissionRecord.PermissionCategory,
                    Description = permissionRecord.Description
                });

                foreach (var role in customerRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.Name))
                        model.Allowed[permissionRecord.Name] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.Name][role.Id] = permissionRecord.RolePermittedFeatures
                        .Any(mapping => mapping.RoleId == role.Id);
                }
            }

            return model;
        }
    }
}
