using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.LabeledGoods.Disabled;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.LabeledGood;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.Shared;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.LabeledGoods;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(LabeledGoodManagementPermission))]
    public class LabeledGoodsController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly IDisabledLabeledGoodsManager _disabledLabeledGoodsManager;
        private readonly ILogger _logger;

        public LabeledGoodsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _disabledLabeledGoodsManager = serviceProvider.GetRequiredService<IDisabledLabeledGoodsManager>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [HttpPost("plant/{plantId}/content")]
        public async Task<IActionResult> SendPlantContentRequestAsync(int plantId)
        {
            var posContentRequestResult = await _nasladdinApiClient.SendPosContentRequestAsync(plantId);

            return posContentRequestResult.ToActionResult();
        }

        [HttpGet("/api/pointsOfSales/{posId}/labeledGoods/untied")]
        public async Task<IActionResult> GetAsync(int posId)
        {
            var labeledGoodsUntiedFromGood =
                await UnitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(posId);

            if (labeledGoodsUntiedFromGood == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о непривязаной метке не найдена" });

            var dtos = labeledGoodsUntiedFromGood.Select(Mapper.Map<LabeledGoodDto>);

            return Ok(dtos);
        }

        [HttpPost("/api/pointsOfSales/{posId}/labeledGoods/untied")]
        public async Task<IActionResult> TieLabelsToGoodAsync(int posId, [FromBody] LabelsToGoodDto labelsToGoodDto)
        {
            _logger.LogFormattedInfo($"{nameof(TieLabelsToGoodAsync)} initiated with parameters: {{0}}.", labelsToGoodDto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var labelsToTie = new SortedSet<string>(labelsToGoodDto.Labels);

            var untiedLabeledGoods = await UnitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(posId);

            var labeledGoodsToTie = untiedLabeledGoods
                .Where(lg => labelsToTie.Contains(lg.Label))
                .ToImmutableList();

            labeledGoodsToTie.ForEach(lg =>
            {
                var expirationPeriod = new ExpirationPeriod(
                    labelsToGoodDto.ManufactureDateOrToday,
                    labelsToGoodDto.ExpirationDateOrToday
                );
                var price = new LabeledGoodPrice(labelsToGoodDto.Price.Value, labelsToGoodDto.CurrencyId.Value);
                lg.TieToGood(labelsToGoodDto.GoodId.Value, price, expirationPeriod);
            });

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("/api/pointsOfSales/{posId}/labeledGoods/untied/blocked")]
        public async Task<IActionResult> DisableLabelsAsync(int posId, [FromBody] LabelsDto labelsDto)
        {
            _logger.LogFormattedInfo($"{nameof(DisableLabelsAsync)} initiated with parameters: {{0}}.", labelsDto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var labeledGoods = await UnitOfWork.LabeledGoods.GetEnabledByLabelsAsync(labelsDto.Labels);

            labeledGoods.ForEach(lg => lg.Disable());

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("/api/labeledGoods/enabled")]
        public async Task<IActionResult> EnableAsync([FromBody] LabeledGoodIdsDto labeledGoodIdsDto)
        {
            _logger.LogFormattedInfo($"{nameof(EnableAsync)} initiated with parameters: {{0}}.", labeledGoodIdsDto);

            await _disabledLabeledGoodsManager.EnableAsync(labeledGoodIdsDto.Values);

            return Ok(labeledGoodIdsDto);
        }

        [HttpGet("/api/labeledGoods/disabled")]
        public async Task<IActionResult> GetDisabledLabeledGoodsAsync()
        {
            var disabledLabeledGoodsGroupedByPos =
                await _disabledLabeledGoodsManager.GetDisabledLabeledGoodsGroupedByPointsOfSaleAsync();

            var disabledLabeledGoodsGroupedByPosDto =
                disabledLabeledGoodsGroupedByPos
                    .Select(Mapper.Map<PosGroupDto<LabeledGoodDto>>)
                    .ToImmutableList();

            return Ok(disabledLabeledGoodsGroupedByPosDto);
        }

        [HttpPut]
        public async Task<IActionResult> EditLabeledGoodAsync([FromBody] LabeledGoodFormViewModel viewModel)
        {
            _logger.LogFormattedInfo($"{nameof(EditLabeledGoodAsync)} initiated with parameters: {{0}}.", viewModel);

            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var labeledGood = await UnitOfWork.LabeledGoods.GetEnabledAsync(viewModel.Id);
            if (labeledGood == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о метке не найдена" });

            if (viewModel.GoodId.HasValue)
            {
                var good = await UnitOfWork.Goods.GetAsync(viewModel.GoodId.Value);
                if (good == null)
                    return BadRequest(new ErrorResponseDto { Error = "Информация о товаре не найдена" });
            }

            if (viewModel.GoodId.HasValue &&
                viewModel.Price.HasValue &&
                viewModel.CurrencyId.HasValue &&
                !string.IsNullOrWhiteSpace(viewModel.ManufactureDate) &&
                !string.IsNullOrWhiteSpace(viewModel.ExpirationDate))
            {
                var expirationPeriod = viewModel.ExpirationPeriod;
                var price = new LabeledGoodPrice(viewModel.Price.Value, viewModel.CurrencyId.Value);
                labeledGood.TieToGood(viewModel.GoodId.Value, price, expirationPeriod);
            }

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("untied/{id}")]
        public async Task<IActionResult> UntieLabelFromGoodAsync(int id)
        {
            _logger.LogInfo($"{nameof(UntieLabelFromGoodAsync)} initiated for labeled good with id {id}.");

            var labeledGood = await UnitOfWork.LabeledGoods.GetByIdAsync(id);
            if (labeledGood == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о метке не найдена" });

            labeledGood.UntieFromGood();

            await UnitOfWork.CompleteAsync();

            return Ok();
        }
    }
}