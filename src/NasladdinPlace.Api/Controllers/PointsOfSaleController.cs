using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Api.Dtos.PosAntennasOutputPower;
using NasladdinPlace.Api.Dtos.PosImage;
using NasladdinPlace.Api.Dtos.PosOperation;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.Brand;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Application.Services.PosOperations.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.DistancesToPointsOfSale;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter;
using NasladdinPlace.Core.Utils.ActionExecution.Models;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.Dtos.Purchase;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Configuration.Reader;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/plants")]
    [Route("api/pointsOfSale")]
    public class PointsOfSaleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPosInteractor _posInteractor;
        private readonly IDistancesToPointsOfSaleCalculator _distancesToPointsOfSaleCalculator;
        private readonly ILogger _logger;
        private readonly IPosRemoteControllerFactory _posRemoteControllerFactory;
        private readonly ILimitedFrequencyActionExecutor _limitedFrequencyActionExecutor;
        private readonly IPurchaseManager _purchaseManager;
        private readonly IPosOperationsAppService _posOperationsAppService;
        private readonly string _baseUrl;
        private readonly RoleManager<Role> _roleManager;

        public PointsOfSaleController(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _posInteractor = serviceProvider.GetRequiredService<IPosInteractor>();
            _distancesToPointsOfSaleCalculator = serviceProvider.GetRequiredService<IDistancesToPointsOfSaleCalculator>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _posRemoteControllerFactory = serviceProvider.GetRequiredService<IPosRemoteControllerFactory>();
            _limitedFrequencyActionExecutor = serviceProvider.GetRequiredService<ILimitedFrequencyActionExecutor>();
            _purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();
            _posOperationsAppService = serviceProvider.GetRequiredService<IPosOperationsAppService>();
            _baseUrl = serviceProvider.GetRequiredService<IConfigurationReader>().GetJwtBearerOptionsAudience();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPos(int id)
        {
            var pos = await _unitOfWork.PointsOfSale.GetByIdIncludingCityAsync(id);

            var result = Mapper.Map<PosDto>(pos);

            return pos == null
                ? (IActionResult)NotFound()
                : Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePos()
        {
            var operation = await _unitOfWork.PosOperations.GetLatestUnpaidOfUserAsync(GetUserId());

            if (operation == null)
                return NotFound();

            var pos = await _unitOfWork.PointsOfSale.GetByIdIncludingCityAsync(operation.PosId);
            var posDto = Mapper.Map<PosDto>(pos);

            posDto.DoorsState = _unitOfWork.PosRealTimeInfos.GetById(pos.Id).DoorsState;

            return Ok(posDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetPointsOfSale()
        {
            var pointsOfSale = (await _unitOfWork.PointsOfSale.GetAllIncludingCityAndImagesAsync()).Select(Mapper.Map<PosDto>).ToList();

            AddAdditionalInfoToPoses(pointsOfSale);

            return Ok(pointsOfSale);
        }

        [HttpGet("byRole")]
        public async Task<IActionResult> GetPointsOfSaleByRole()
        {
            var roleIds = await GetRoleIdsAsync();
            var pointsOfSaleDtos = new List<PosDto>();

            foreach (var roleId in roleIds)
            {
                var poses = await _unitOfWork.PointsOfSale.GetAllAvailablePosesByRoleAsync(roleId);
                pointsOfSaleDtos.AddRange(Mapper.Map<List<PosDto>>(poses));
            }

            AddAdditionalInfoToPoses(pointsOfSaleDtos);

            return Ok(pointsOfSaleDtos);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public async Task<IActionResult> DeletePos(int id)
        {
            var pos = await _unitOfWork.PointsOfSale.GetByIdIncludingAllowedOperationModesAsync(id);
            if (pos == null)
                return NotFound();

            _unitOfWork.PointsOfSale.Remove(pos);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }

        [HttpPost("plantsWithDistances")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPointsOfSaleWithDistancesAsync([FromBody] LocationDto dto)
        {
            var fromLocation = new Location(dto.Latitude ?? 0, dto.Longitude ?? 0);
            var toPosItems = await _unitOfWork.PointsOfSale.GetAllIncludingCityAndImagesAsync();

            var distancesToPlants = _distancesToPointsOfSaleCalculator.CalculateDistances(fromLocation, toPosItems);

            var posItemsDtos = distancesToPlants.Select(dtp =>
            {
                var posDto = Mapper.Map<PosDto>(dtp.Pos);
                posDto.DistanceInKmRelativeToUser = dtp.RoundedDistanceInKm;
                posDto.Images = posDto.Images.Select(i => new PosImageDto
                {
                    Id = i.Id,
                    PosId = i.PosId,
                    ImagePath = ConfigurationReaderExt.CombineUrlParts(_baseUrl, i.ImagePath)
                }).ToImmutableList();
                return posDto;
            })
            .OrderBy(p => p.DistanceInKmRelativeToUser)
            .ToImmutableList();

            return Ok(posItemsDtos);
        }

        [HttpPost("nearest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearestPos([FromBody] LocationDto dto)
        {
            var fromLocation = new Location(dto.Latitude ?? 0, dto.Longitude ?? 0);
            var toNearestPos = await _unitOfWork.PointsOfSale.GetNearestAsync(fromLocation);

            var distanceToPlant = _distancesToPointsOfSaleCalculator.CalculateDistance(fromLocation, toNearestPos);

            var posDto = Mapper.Map<PosDto>(toNearestPos);

            posDto.DistanceInKmRelativeToUser = distanceToPlant.RoundedDistanceInKm;

            return Ok(posDto);
        }

        [HttpPost("active/anotherDoor")]
        public IActionResult ContinuePurchase()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            _posOperationsAppService.ContinuePurchaseAsync(userId);

            return Ok();
        }

        [HttpPost("active/door")]
        public async Task<IActionResult> InitiatePurchaseAsync([FromBody] PurchaseInitiationRequestDto dto)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var purchaseInitiationResult =
                await _purchaseManager.InitiateAsync(new PurchaseInitiationParams(userId, dto.QrCode)
                {
                    Brand = HttpContext.Request.GetBrandHeaderValue()
                });

            if (!purchaseInitiationResult.Succeeded)
            {
                _logger.LogError($"An error occurred during initialization of purchase of user {userId}:" +
                              $" {purchaseInitiationResult.Error}. " +
                              $"Initiation status is {purchaseInitiationResult.Status.ToString()}."
                );
                return BadRequest(purchaseInitiationResult.Error);
            }

            var pos = purchaseInitiationResult.PosOperation.Pos;

            _logger.LogInfo($"The user {userId} has been successfully initiated operation with POS {pos.Id} by " +
                                $"providing QR code {dto.QrCode}.");

            var posDto = Mapper.Map<PosDto>(pos);

            return Ok(posDto);
        }

        [HttpDelete("active/doors")]
        public IActionResult InitiatePurchaseCompletion()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            _posOperationsAppService.InitiatePurchaseCompletionAsync(userId);

            return Ok();
        }

        [HttpPost("{posId}/rightDoor")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public Task<IActionResult> OpenRightDoor(int posId, [FromBody] PosOperationModeDto dto)
        {
            return InitiatePosOperationAsync(posId, PosDoorPosition.Right, dto.Mode.Value);
        }

        [HttpPost("{posId}/leftDoor")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public Task<IActionResult> OpenLeftDoor(int posId, [FromBody] PosOperationModeDto dto)
        {
            return InitiatePosOperationAsync(posId, PosDoorPosition.Left, dto.Mode.Value);
        }

        [HttpPost("{posId}/antennasOutputPower")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public IActionResult UpdateAntennasOutputPower(
            int posId, [FromBody] PosAntennasOutputPowerDto antennasOutputPowerDto)
        {
            var posRemoteController = _posRemoteControllerFactory.Create(posId);

            var updatePosAntennasOutputPowerTask =
                posRemoteController.SetAntennasOutputPowerAsync(antennasOutputPowerDto.OutputPower.Value);
            updatePosAntennasOutputPowerTask.ContinueWith(t =>
            {
                t.Wait();
                return posRemoteController.RequestAntennasOutputPowerAsync();
            }).Wait();

            return Ok();
        }

        [HttpDelete("{posId}/doors")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public async Task<IActionResult> CloseDoors(int posId)
        {
            await _posInteractor.SendOperationCompletionRequestAsync(posId);

            return Ok();
        }

        [HttpGet("{posId}/content")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public async Task<IActionResult> GetContent(int posId)
        {
            var posRemoteController = _posRemoteControllerFactory.Create(posId);
            await posRemoteController.RequestAccountingBalancesAsync();

            return Ok();
        }

        [HttpGet("{posId}/realTimeInfo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosRealTimeInfoAsync(int posId)
        {
            var posRemoteController = _posRemoteControllerFactory.Create(posId);

            var antennasPowerRequestActionIdentifier =
                nameof(INasladdinWebSocketMessageSender.RequestPosAntennasOutputPowerAsync) + posId;
            var antennasPowerRequestFrequencyInfo = new ActionExecutionFrequencyInfo(
                antennasPowerRequestActionIdentifier,
                TimeSpan.FromMinutes(10)
            );

            _limitedFrequencyActionExecutor.TryExecute(
                action: () => { posRemoteController.RequestAntennasOutputPowerAsync(); },
                actionExecutionFrequencyInfo: antennasPowerRequestFrequencyInfo
            );

            var posRealTimeInfo = _unitOfWork.PosRealTimeInfos.GetById(posId);

            var result = Mapper.Map<PosRealTimeInfoDto>(posRealTimeInfo);

            var hardToDetectLabeledGoods =
                await _unitOfWork.LabeledGoods.GetByLabelsAsync(posRealTimeInfo.HardToDetectLabels);

            result.HardToDetectLabels = hardToDetectLabeledGoods
                .Where(lg => !lg.IsDisabled)
                .Select(Mapper.Map<HardToDetectLabelDto>)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{posId}/lastReceivedWsMessage")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public async Task<string> GetPosLastReceivedWsMessage(int posId)
        {
            return await Task.Run(() =>
            {
                var posRealTimeInfo = _unitOfWork.PosRealTimeInfos.GetById(posId);

                var lastReceivedWsMessage = posRealTimeInfo.LastReceivedWsMessage;
                
                return lastReceivedWsMessage.IsDeactivatedPosReceivingWsMessages ?
                    lastReceivedWsMessage.ToString() :
                    string.Empty;
            });
        }

        [HttpGet("{posId}/temperatureDetails")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public IActionResult GetTemperatureDetails(int posId)
        {
            var posTemperatures = _unitOfWork.PosTemperatures.GetByPosId(posId);
            var result = posTemperatures.Select(Mapper.Map<PosTemperatureDto>).ToList();
            result.ForEach(r => r.PosId = posId);

            return Ok(result);
        }

        private async Task<IActionResult> InitiatePosOperationAsync(int posId, PosDoorPosition doorPosition, PosMode mode)
        {
            var operationInitiationParams = GetPosOperationInitiationParams(posId, mode, doorPosition);

            var result = await _posInteractor.InitiatePosOperationAsync(operationInitiationParams);

            return result.Succeeded
                ? (IActionResult)Ok()
                : BadRequest(result.Error);
        }

        private PosOperationInitiationParams GetPosOperationInitiationParams(
            int posId, PosMode mode, PosDoorPosition doorPosition)
        {
            var userId = GetUserId();
            switch (mode)
            {
                case PosMode.Purchase:
                    return PosOperationInitiationParams.ForPurchase(userId, posId, Brand.Invalid);
                case PosMode.GoodsIdentification:
                    return PosOperationInitiationParams.ForGoodsIdentification(userId, posId, doorPosition);
                case PosMode.GoodsPlacing:
                    return PosOperationInitiationParams.ForGoodsPlacing(userId, posId, doorPosition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, $"{nameof(PosMode)} has not been supported yet.");
            }
        }

        private int GetUserId()
        {
            return _userManager.GetUserIdAsInt(User);
        }

        private async Task<List<int>> GetRoleIdsAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roleNames = await _userManager.GetRolesAsync(user);

            var roleIds = new List<int>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                    roleIds.Add(role.Id);
            }

            return roleIds;
        }

        private void AddAdditionalInfoToPoses(List<PosDto> pointsOfSale)
        {
            foreach (var posDto in pointsOfSale)
            {
                var posRealTimeInfo = _unitOfWork.PosRealTimeInfos.GetById(posDto.Id);
                posDto.Status = posRealTimeInfo.ConnectionStatus;
                posDto.IpAddresses = posRealTimeInfo.IpAddresses.Select(ip => ip.ToString());
                posDto.Version = posRealTimeInfo.Version;
            }
        }
    }
}