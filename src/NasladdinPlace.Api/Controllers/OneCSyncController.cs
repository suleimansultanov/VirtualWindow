using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.OneCSync;
using NasladdinPlace.Api.Services.OneCSync;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class OneCSyncController : Controller
    {
        private readonly IOneCSyncService _oneCSyncService;

        public OneCSyncController(IOneCSyncService oneCSyncService)
        {
            if (oneCSyncService == null)
                throw new ArgumentNullException(nameof(oneCSyncService));

            _oneCSyncService = oneCSyncService;
        }

        [HttpGet("GetPurchaseList")]
        public async Task<IActionResult> GetPurchasesListAsync(DateRangeDto dtoRange)
        {
            if (!IsDateRangeValid(dtoRange, out DateTimeRange dateRange))
            {
                return BadRequest($"Невалидное значение параметра {nameof(dateRange)}.");
            }

            var oneCSyncOperationResult = await _oneCSyncService.GetPurchasesListByDateRangeAsync(dateRange);

            if (oneCSyncOperationResult.Succeeded)
                return Ok(oneCSyncOperationResult.Value);

            return BadRequest(oneCSyncOperationResult.Error);
        }

        [HttpGet("purchaseList")]
        public async Task<IActionResult> GetVersionTwoPurchasesListAsync(DateRangeDto dtoRange)
        {
            if (!IsDateRangeValid(dtoRange, out DateTimeRange dateRange))
            {
                return BadRequest($"Невалидное значение параметра {nameof(dateRange)}.");
            }

            var oneCSyncOperationResult = await _oneCSyncService.GetVersionTwoPurchasesListByDateRangeAsync(dateRange);

            if (oneCSyncOperationResult.Succeeded)
                return Ok(oneCSyncOperationResult.Value);

            return BadRequest(oneCSyncOperationResult.Error);
        }

        [HttpGet("inventoryBalances")]
        public IActionResult InventoryBalances()
        {
            var oneCSyncInventoryBalancesResult = _oneCSyncService.GetInventoryBalances();

            if (oneCSyncInventoryBalancesResult.Succeeded)
                return Ok(oneCSyncInventoryBalancesResult.Value);

            return BadRequest(oneCSyncInventoryBalancesResult.Error);
        }

        [HttpGet("goodsMoving")]
        public async Task<IActionResult> GoodsMovingAsync(DateRangeDto dtoRange)
        {
            if (!IsDateRangeValid(dtoRange, out DateTimeRange dateRange))
            {
                return BadRequest($"Невалидное значение параметра {nameof(dateRange)}.");
            }

            var oneCSyncGoodsMovingResult = await _oneCSyncService.GetDocumentGoodsMovingAsync(dateRange);

            if (oneCSyncGoodsMovingResult.Succeeded)
                return Ok(oneCSyncGoodsMovingResult.Value);

            return BadRequest(oneCSyncGoodsMovingResult.Error);
        }

        private bool IsDateRangeValid(DateRangeDto dto, out DateTimeRange range)
        {
            range = new DateTimeRange();
            var validDate = new DateTime(2000, 01, 01, 00, 00, 00);
            try
            {
                if (dto.ToDate.HasValue)
                {
                    range = DateTimeRange.From(dto.FromDate, dto.ToDate.Value);
                }
                else
                {
                    range = DateTimeRange.Since(dto.FromDate);
                }

                return range.Start > validDate;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}