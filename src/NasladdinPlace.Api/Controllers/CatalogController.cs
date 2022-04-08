using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.Catalog.Contracts;
using NasladdinPlace.Core.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NasladdinPlace.Dtos.Catalog;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [ApiController]
    [Authorize]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CatalogController(
            ICatalogService catalogService,
            UserManager<ApplicationUser> userManager)
        {
            if (catalogService == null)
                throw new ArgumentNullException(nameof(catalogService));
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            _catalogService = catalogService;
            _userManager = userManager;
        }

        [HttpPost("categoryItems")]
        public async Task<IActionResult> GetCategoryItems([FromBody] CategoryItemsDto categoryItemsDto)
        {
            if (categoryItemsDto.Page <= 0)
                return BadRequest($"{nameof(categoryItemsDto.Page)} must be greater than 0.");

            if (categoryItemsDto.CategoryId < 0)
                return BadRequest($"{nameof(categoryItemsDto.CategoryId)} must be greater or equal 0.");

            if (categoryItemsDto.PosId <= 0)
                return BadRequest($"{nameof(categoryItemsDto.PosId)} must be greater than 0.");

            var pointsOfSaleResult =
                await _catalogService.GetCategoryItemsAsync(categoryItemsDto);

            if (pointsOfSaleResult.Succeeded)
                return Ok(pointsOfSaleResult.Value);

            return BadRequest(pointsOfSaleResult.Error);
        }

        [HttpGet("pointsOfSale")]
        public async Task<IActionResult> GetPointsOfSaleAsync(byte page)
        {
            if (page <= 0)
                return BadRequest($"{nameof(page)} must be greater than 0.");

            var userId = _userManager.GetUserIdAsInt(User);

            var pointsOfSaleResult =
                await _catalogService.GetPointsOfSaleAsync(page, userId);

            if (pointsOfSaleResult.Succeeded)
                return Ok(pointsOfSaleResult.Value);

            return BadRequest(pointsOfSaleResult.Error);
        }

        [HttpGet("posContent")]
        public async Task<IActionResult> GetPosContent(int posId, byte page)
        {
            if (page <= 0)
                return BadRequest($"{nameof(page)} must be greater than 0.");

            if (posId <= 0)
                return BadRequest($"{nameof(posId)} must be greater than 0.");

            var posContentResult =
                await _catalogService.GetPosContentAsync(posId, page);

            if (posContentResult.Succeeded)
                return Ok(posContentResult.Value);

            return BadRequest(posContentResult.Error);
        }

        [HttpGet("virtual/posContent")]
        public async Task<IActionResult> GetVirtualPosContent(byte page)
        {
            if (page <= 0)
                return BadRequest($"{nameof(page)} must be greater than 0.");

            var posContentResult =
                await _catalogService.GetVirtualPosContentAsync(page);

            if (posContentResult.Succeeded)
                return Ok(posContentResult.Value);

            return BadRequest(posContentResult.Error);
        }

        [HttpPost("virtual/categoryItems")]
        public async Task<IActionResult> GetVirtualCategoryItems([FromBody] CategoryItemsDto categoryItemsDto)
        {
            if (categoryItemsDto.Page <= 0)
                return BadRequest($"{nameof(categoryItemsDto.Page)} must be greater than 0.");

            if (categoryItemsDto.CategoryId < 0)
                return BadRequest($"{nameof(categoryItemsDto.CategoryId)} must be greater or equal 0.");

            var categoryItemsResult =
                await _catalogService.GetVirtualCategoryItemsAsync(categoryItemsDto);

            if (categoryItemsResult.Succeeded)
                return Ok(categoryItemsResult.Value);

            return BadRequest(categoryItemsResult.Error);
        }
    }
}