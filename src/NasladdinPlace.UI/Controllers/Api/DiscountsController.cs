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
using NasladdinPlace.UI.Managers.Discounts;
using NasladdinPlace.UI.ViewModels.Discounts;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(ReadOnlyAccess))]
    public class DiscountsController : BaseController
    {
        private readonly IDiscountsManager _discountsManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscountsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _discountsManager = serviceProvider.GetRequiredService<IDiscountsManager>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        [HttpGet("{id}")]
        public IActionResult GetDiscountInfo(int id)
        {
            var discount = UnitOfWork.Discounts.GetByIdIncludePosDiscountsAndRules(id);
            if (discount == null)
                return BadRequest(new ErrorResponseDto { Error = "Информация о скидке не найдена" });

            return JsonContent(new
            {
                result = _discountsManager.GetDiscountViewModel(discount)
            });
        }

        [HttpPost]
        [Permission(nameof(DiscountCrudPermission))]
        public async Task<IActionResult> AddDiscountAsync([FromBody] DiscountInfoViewModel discountInfo)
        {
            if (!_discountsManager.Validate(discountInfo, out var validationError))
                return BadRequest(validationError);

            await _discountsManager.AddAsync(discountInfo);

            return Ok();
        }

        [HttpPut]
        [Permission(nameof(DiscountCrudPermission))]
        public async Task<IActionResult> EditDiscountAsync([FromBody] DiscountInfoViewModel discountInfo)
        {
            if (!_discountsManager.Validate(discountInfo, out var validationError))
                return BadRequest(new { Error = validationError });

            var user = await GetCurrentUserAsync();

            await _discountsManager.UpdateAsync(discountInfo, user);

            return Ok();
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}