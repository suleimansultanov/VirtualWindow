using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.GoodCategories.Contracts;
using NasladdinPlace.UI.ViewModels.GoodCategories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(ReadOnlyAccess))]
    public class GoodCategoriesController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoodCategoryService _goodCategoryService;

        public GoodCategoriesController(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _goodCategoryService = serviceProvider.GetRequiredService<IGoodCategoryService>();
        }

        [HttpPut]
        [Refit.Multipart]
        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> EditCategoryAsync([FromForm] GoodCategoryViewModel viewModel)
        {
            var userId = GetUserIdAsInt(User);

            var editCategoryResult = await _goodCategoryService.EditGoodCategoryAsync(viewModel, userId);

            if (!editCategoryResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = editCategoryResult.Error });

            return Ok();
        }

        [HttpPost]
        [Refit.Multipart]
        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> AddCategoryAsync([FromForm] GoodCategoryViewModel viewModel)
        {
            var userId = GetUserIdAsInt(User);

            var editCategoryResult = await _goodCategoryService.AddGoodCategoryAsync(viewModel, userId);

            if (!editCategoryResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = editCategoryResult.Error });

            return Ok();
        }

        [HttpGet]
        public JsonResult GetBaseApiUrl()
        {
            var getBaseApiUrlResult = _goodCategoryService.GetBaseApiUrl();
            if (getBaseApiUrlResult.Succeeded)
                return Json(new { data = getBaseApiUrlResult.Value });

            return Json(new
            {
                data = "failure",
                error = "Произошла ошибка при получении базового URL-адреса API. Пожалуйста, обратитесь к администратору."
            });
        }

        [HttpGet("defaultImage")]
        public JsonResult GetDefaultImagePath()
        {
            var getDefaultImagePathResult = _goodCategoryService.GetDefaultImagePath();
            if (getDefaultImagePathResult.Succeeded)
                return Json(new { data = getDefaultImagePathResult.Value });

            return Json(new
            {
                data = "failure",
                error = "Произошла ошибка при получении изображения по умолчанию. Пожалуйста, обратитесь к администратору."
            });
        }

        private int GetUserIdAsInt(ClaimsPrincipal principal)
        {
            return int.Parse(_userManager.GetUserId(principal));
        }
    }
}