using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.Shared;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.Goods.Contracts;
using NasladdinPlace.UI.ViewModels.Goods;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class GoodsController : BaseController
    {
        private readonly IGoodService _goodService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoodsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _goodService = serviceProvider.GetRequiredService<IGoodService>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        [HttpGet]
        public async Task<IActionResult> GetGoodsAsync()
        {
            var result = (await UnitOfWork.Goods.GetAllPublishedAsync()).Select(Mapper.Map<GoodDto>);

            return Ok(result);
        }

        [HttpPost]
        [Refit.Multipart]
        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> AddGoodAsync([FromForm] GoodsFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var userId = GetUserIdAsInt(User);

            var addGoodResult = await _goodService.AddGoodAsync(viewModel, userId);

            if (!addGoodResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = addGoodResult.Error });

            return Ok(addGoodResult.Value);
        }

        [HttpPut]
        [Refit.Multipart]
        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> EditGoodAsync([FromForm] GoodsFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var userId = GetUserIdAsInt(User);

            var editGoodResult = await _goodService.EditGoodAsync(viewModel, userId);

            if (!editGoodResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = editGoodResult.Error });

            return Ok();
        }

        [HttpGet("apiBaseUrl")]
        public JsonResult GetBaseApiUrl()
        {
            var getBaseApiUrlResult = _goodService.GetBaseApiUrl();
            if (getBaseApiUrlResult.Succeeded)
                return Json(new { data = getBaseApiUrlResult.Value });

            return Json(new
            {
                data = "failure",
                error = getBaseApiUrlResult.Error
            });
        }

        [HttpDelete("{id}")]
        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> DeleteGoodAsync(int id)
        {
            var userId = GetUserIdAsInt(User);

            var deleteGoodResult = await _goodService.DeleteGoodAsync(id, userId);

            if (!deleteGoodResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = deleteGoodResult.Error });

            return Ok();
        }

        private int GetUserIdAsInt(ClaimsPrincipal principal)
        {
            return int.Parse(_userManager.GetUserId(principal));
        }
    }
}