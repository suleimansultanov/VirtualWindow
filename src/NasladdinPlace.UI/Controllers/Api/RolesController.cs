using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Resources.Views.Users;
using NasladdinPlace.UI.ViewModels.Roles;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;
using NasladdinPlace.Logging;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class RolesController : BaseController
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger _logger;

        public RolesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectListRolesAsync()
        {
            var rolesSelectList = await GetRolesAsSelectListAsync();
            return Ok(rolesSelectList);
        }

        [HttpPost]
        [Permission(nameof(AclManagementPermission))]
        public async Task<IActionResult> AddRole([FromBody] RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var role = Role.FromNameAndDescription(viewModel.Name, viewModel.Description);

            await _roleManager.CreateAsync(role);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Permission(nameof(AclManagementPermission))]
        public async Task<IActionResult> RemoveRole(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if(role == null)
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Такой роли не существует."
                });

            try
            {
                var deletionResult = await _roleManager.DeleteAsync(role);

                return deletionResult.Succeeded
                    ? Ok()
                    : (IActionResult)BadRequest(new ErrorsResponseDto
                    {
                        Errors = deletionResult.Errors
                            .Select(x => x.Description)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{User.Identity.Name} tried to delete role {role.Name}. Verbose error: {ex}");
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Невозможно удалить роль, пожалуйста обратитесь к администратору."
                });
            }
        }

        private async Task<SelectList> GetRolesAsSelectListAsync()
        {
            var roles = await UnitOfWork.Roles.GetAllAsync();
            return new SelectList(
                roles
                    .Select(r => new SelectListItem
                    {
                        Text = _createUserPartial.ResourceManager.TryGetResourceValue(r.Name, out var resourceValue)
                            ? resourceValue
                            : r.Name,
                        Value = r.Id.ToString()
                    }), "Value", "Text");
        }
    }
}