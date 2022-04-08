using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class LabeledGoodsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabeledGoodsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] LabeledGoodDto dto)
        {
            var good = await _unitOfWork.Goods.GetAsync(dto.GoodId ?? 0);
            if (good == null)
                return BadRequest($"Good with id {dto.GoodId} does not exist.");
            if (!dto.PosId.HasValue)
                return BadRequest("Labeled good must be in pos.");

            var labeledGood = LabeledGood.OfPos(dto.PosId.Value, dto.Label);
            if (dto.GoodId.HasValue && dto.Price.HasValue && dto.CurrencyId.HasValue)
            {
                var price = new LabeledGoodPrice(dto.Price.Value, dto.CurrencyId.Value);
                labeledGood.TieToGood(dto.GoodId.Value, price, dto.ExpirationPeriod);
            }

            if (dto.PosOperationId.HasValue)
            {
                labeledGood.MarkAsUsedInPosOperation(dto.PosOperationId.Value);
            }
            _unitOfWork.LabeledGoods.Add(labeledGood);
            _unitOfWork.CompleteAsync().Wait();

            return StatusCode(StatusCodes.Status201Created, labeledGood.Id);
        }

        [HttpDelete("/api/shops/{shopId:int}/LabelGoods/{labeledGoodId:int}/good")]
        public async Task<IActionResult> UntieLabelFromGood(int labeledGoodId, int shopId)
        {
            var labeledGood = await _unitOfWork.LabeledGoods.GetEnabledAsync(labeledGoodId);

            if (labeledGood == null)
            {
                return NotFound();
            }

            labeledGood.UntieFromGood();

            await _unitOfWork.CompleteAsync();

            return Ok(labeledGoodId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] LabeledGoodDto dto)
        {
            var labeledGood = await _unitOfWork.LabeledGoods.GetEnabledAsync(dto.Id);
            if (labeledGood == null)
                return NotFound();

            if (dto.GoodId.HasValue)
            {
                var good = await _unitOfWork.Goods.GetAsync(dto.GoodId.Value);
                if (good == null)
                    return BadRequest($"Good with id {dto.GoodId} does not exist.");
            }

            if (dto.GoodId.HasValue)
            {
                var good = await _unitOfWork.Goods.GetAsync(dto.GoodId.Value);

                if (good == null)
                    return BadRequest();
            }

            if (dto.GoodId.HasValue &&
                dto.Price.HasValue &&
                dto.CurrencyId.HasValue &&
                !string.IsNullOrWhiteSpace(dto.ManufactureDate) &&
                !string.IsNullOrWhiteSpace(dto.ExpirationDate))
            {
                var price = new LabeledGoodPrice(dto.Price.Value, dto.CurrencyId.Value);
                var expirationPeriod = dto.ExpirationPeriod;
                labeledGood.TieToGood(dto.GoodId.Value, price, expirationPeriod);
            }
            else if (
              !string.IsNullOrWhiteSpace(dto.ManufactureDate) &&
              !string.IsNullOrWhiteSpace(dto.ExpirationDate))
            {
                labeledGood.UpdateExpirationPeriod(dto.ExpirationPeriod);
            }

            if (!dto.PosId.HasValue)
            {
                labeledGood.MarkAsNotBelongingToUserOrPos();
            }

            await _unitOfWork.CompleteAsync();

            return Ok(dto.Id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var labeledGood = await _unitOfWork.LabeledGoods.GetEnabledAsync(id);
            return labeledGood == null
                ? (IActionResult)NotFound()
                : Ok(ToDto(labeledGood));
        }

        [HttpGet("/api/shops/{shopId:int}/labeledGoods")]
        public async Task<IActionResult> GetInPosAsync(int shopId)
            => Ok((await _unitOfWork.LabeledGoods.GetEnabledInPosAsync(shopId)).Select(ToDto));

        [HttpGet("/api/shops/{shopId:int}/labeledGoods/overdue")]
        public async Task<IActionResult> GetOverdueInPosAsync(int shopId)
            => Ok((await _unitOfWork.LabeledGoods.GetEnabledOverdueInPosAsync(shopId, TimeSpan.FromDays(1))).Select(ToDto));

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
            => Ok((await _unitOfWork.LabeledGoods.GetAllEnabledAsync()).Select(ToDto));

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetInStockAsync()
            => Ok((await _unitOfWork.LabeledGoods.GetEnabledInStockAsync()).Select(ToDto));

        [HttpGet("/api/shops/{shopId:int}/labeledGoods/untiedFromGood")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUntiedFromGoodByPos(int shopId)
        {
            var labeledGoodsUntiedFromGood =
                await _unitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(shopId);
            var dtos = labeledGoodsUntiedFromGood.Select(ToDto);
            return Ok(dtos);
        }

        [Route("/api/shops/{shopId:int}/labeledGoods/untied")]
        [HttpPost]
        public async Task<IActionResult> TieLabelsOfPosToGood(int shopId, [FromBody] LabelsToGoodDto labelsToGoodDto)
        {
            var labelsToTie = new SortedSet<string>(labelsToGoodDto.Labels);

            var untiedLabeledGoods = await _unitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(shopId);
            var labeledGoodsToTie = untiedLabeledGoods
                .Where(lg => labelsToTie.Contains(lg.Label))
                .ToImmutableList();

            labeledGoodsToTie.ForEach(lg =>
            {
                var price = new LabeledGoodPrice(lg.Price.Value, lg.CurrencyId.Value);
                var expirationPeriod = new ExpirationPeriod(
                    labelsToGoodDto.ManufactureDateOrToday,
                    labelsToGoodDto.ExpirationDateOrToday
                );
                lg.TieToGood(labelsToGoodDto.GoodId.Value, price, expirationPeriod);
            });
            await _unitOfWork.CompleteAsync();

            return Ok();
        }

        [Route("/api/shops/{shopId:int}/labeledGoods/untied/blocked")]
        [HttpPost]
        public async Task<IActionResult> BlockLabels(int shopId, [FromBody] LabelsDto labelsDto)
        {
            var labeledGoods = await _unitOfWork.LabeledGoods.GetEnabledByLabelsAsync(labelsDto.Labels);

            labeledGoods.ForEach(lg => lg.Disable());

            await _unitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var labeledGood = await _unitOfWork.LabeledGoods.GetEnabledAsync(id);
            if (labeledGood == null)
                return NotFound();

            _unitOfWork.LabeledGoods.Remove(labeledGood.Id);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }

        private static LabeledGoodDto ToDto(LabeledGood labeledGood)
        {
            return Mapper.Map<LabeledGoodDto>(labeledGood);
        }
    }
}

