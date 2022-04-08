using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Utilities.EnumHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.LabeledGoods;
using NasladdinPlace.Core.Services.LabeledGoods.Partner.Contracts;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/LabeledGoods/Partner")]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class LabeledGoodsPartnerController : Controller
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILabeledGoodPartnerInfoService _labeledGoodPartnerInfoService;

        public LabeledGoodsPartnerController(IUnitOfWorkFactory unitOfWorkFactory, ILabeledGoodPartnerInfoService labeledGoodPartnerInfoService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _labeledGoodPartnerInfoService = labeledGoodPartnerInfoService;
        }

        [HttpPost("group")]
        public async Task<IActionResult> AddAsync([FromBody] List<LabeledGoodPartnerDto> dtos)
        {
            var labeledGoods = new List<LabeledGood>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                unitOfWork.BeginTransaction(); // TODO: обернуть транзакцию в using

                try
                {
                    foreach (var dto in dtos)
                    {
                        var existingLabeledGood = await unitOfWork.LabeledGoods.GetByLabelAsync(dto.Label);
                        if (existingLabeledGood != null)
                            return BadRequest($"Labeled good with label {dto.Label} exists.");

                        if (dto.GoodId.HasValue && !await IsGoodPublished(unitOfWork, dto.GoodId.Value))
                            return BadRequest($"Good with Id = {dto.GoodId} isn't published. Publish good and try again.");

                        var labeledGoodPartnerInfo = Mapper.Map<LabeledGoodPartnerInfo>(dto);
                        var labeledGood = LabeledGood.FromLabel(dto.Label);
                        var result = _labeledGoodPartnerInfoService.Add(labeledGoodPartnerInfo, labeledGood);

                        if (!result.Succeeded)
                            return BadRequest(result.Error);

                        unitOfWork.LabeledGoods.Add(labeledGood);
                        labeledGoods.Add(labeledGood);
                    }

                    await unitOfWork.CompleteAsync();
                    unitOfWork.CommitTransaction();
                }
                catch
                {
                    unitOfWork.RollbackTransaction();
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status201Created, labeledGoods);
        }

        [HttpPut("group")]
        public async Task<IActionResult> UpdateAsync([FromBody] List<LabeledGoodPartnerDto> dtos)
        {
            var ids = new List<int>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                unitOfWork.BeginTransaction(); // TODO: обернуть транзакцию в using

                try
                {
                    foreach (var dto in dtos)
                    {
                        var labeledGood = await unitOfWork.LabeledGoods.GetEnabledAsync(dto.Id);
                        if (labeledGood == null)
                            return NotFound($"LabeledGood with id {dto.Id} does not exist.");

                        if (dto.GoodId.HasValue)
                        {
                            var good = await unitOfWork.Goods.GetAsync(dto.GoodId.Value);
                            if (good == null)
                                return BadRequest($"Good with id {dto.GoodId} does not exist.");

                            if(!good.IsPublished)
                                return BadRequest($"Good with Id = {dto.GoodId} isn't published. Publish good and try again.");
                        }

                        var labeledGoodPartnerInfo = Mapper.Map<LabeledGoodPartnerInfo>(dto);
                        var result = _labeledGoodPartnerInfoService.Update(labeledGoodPartnerInfo, labeledGood);

                        if (!result.Succeeded)
                            return BadRequest(result.Error);

                        ids.Add(result.Value.Id);
                    }

                    await unitOfWork.CompleteAsync();
                    unitOfWork.CommitTransaction();
                }
                catch
                {
                    unitOfWork.RollbackTransaction();
                    throw;
                }
            }

            return Ok(ids);
        }

        [HttpPost("byLabels")]
        public async Task<IActionResult> GetAsync([FromBody] List<string> labels)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var labeledGoods = await unitOfWork.LabeledGoods.GetByLabelsAsync(labels);

                return labeledGoods.Count == 0
                    ? (IActionResult)NotFound()
                    : Ok(labeledGoods.Select(ToDto));
            }
        }

        [HttpPost("byIds")]
        public async Task<IActionResult> GetAsync([FromBody] List<int> ids)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var labeledGoods = await unitOfWork.LabeledGoods.GetByIdsAsync(ids);

                return labeledGoods.Count == 0
                    ? (IActionResult)NotFound()
                    : Ok(labeledGoods.Select(ToDto));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var labeledGoods = (await unitOfWork.LabeledGoods.GetAllEnabledAsync()).Select(ToDto);
                return Ok(labeledGoods);
            }
        }

        private static LabeledGoodPartnerDto ToDto(LabeledGood labeledGood)
        {
            var labeledGoodDto = Mapper.Map<LabeledGoodPartnerDto>(labeledGood);

            if (labeledGood.PosOperationId == null && labeledGood.PosId == null)
            {
                labeledGoodDto.CanBeDeleted = true;
                return labeledGoodDto;
            }

            if (labeledGood.PosOperationId != null)
            {
                labeledGoodDto.CanBeDeleted = false;
                labeledGoodDto.CannotBeDeletedReason = LabeledGoodStatus.GoodHasAlreadyBeenSold.GetDescription();
                return labeledGoodDto;
            }

            labeledGoodDto.CanBeDeleted = false;
            labeledGoodDto.CannotBeDeletedReason = LabeledGoodStatus.GoodHasAlreadyInPointOfSale.GetDescription();

            return labeledGoodDto;
        }

        private async Task<bool> IsGoodPublished(IUnitOfWork unitOfWork, int goodId)
        {
             var good = await unitOfWork.Goods.GetAsync(goodId);
             return good.IsPublished;
        }
    }
}

