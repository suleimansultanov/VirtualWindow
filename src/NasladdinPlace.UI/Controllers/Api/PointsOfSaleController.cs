using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Pos.State.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.PosOperation;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route("api/plants")]
    [Route("api/pointsOfSale")]
    public class PointsOfSaleController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly IConfigurationManager _configurationManager;
        private readonly IPosEquipmentStateManager _posEquipmentStateManager;
        private readonly PosStateChartSettings _chartSettings;

        public PointsOfSaleController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _configurationManager = serviceProvider.GetRequiredService<IConfigurationManager>();
            _posEquipmentStateManager = serviceProvider.GetRequiredService<IPosEquipmentStateManager>();
            _chartSettings = serviceProvider.GetRequiredService<IPosStateSettingsProvider>().GetChartSettings();
        }

        [HttpDelete("{id}")]
        [Permission(nameof(DeletePosPermission))]
        public async Task<IActionResult> DeletePosAsync(int id)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdIncludingAllowedOperationModesAsync(id);

            if (pos == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о витрине не найдена" });

            UnitOfWork.PointsOfSale.Remove(pos);
            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("{posId}/rightDoor")]
        [Permission(nameof(PosDoorsManagementPermission))]
        public async Task<IActionResult> OpenRightDoorAsync(
            int posId, [FromBody] PosOperationModeDto posOperationModeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _nasladdinApiClient.OpenRightPosDoorAsync(posId, posOperationModeDto);

            return result.ToActionResult();
        }

        [HttpPost("{posId}/leftDoor")]
        [Permission(nameof(PosDoorsManagementPermission))]
        public async Task<IActionResult> OpenLeftDoorAsync(
            int posId, [FromBody] PosOperationModeDto posOperationModeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _nasladdinApiClient.OpenLeftPosDoorAsync(posId, posOperationModeDto);

            return result.ToActionResult();
        }

        [HttpPost("{posId}/antennasOutputPower")]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public async Task<IActionResult> UpdateAntennasOutputPowerAsync(
            int posId, [FromBody] PosAntennasOutputPowerDto posAntennasOutputPowerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _nasladdinApiClient.UpdateAntennasOutputPowerAsync(posId, posAntennasOutputPowerDto);

            return result.ToActionResult();
        }

        [HttpDelete("{posId}/doors")]
        [Permission(nameof(PosDoorsManagementPermission))]
        public async Task<IActionResult> CloseDoorsAsync(int posId)
        {
            var result = await _nasladdinApiClient.ClosePosDoorsAsync(posId);

            return result.ToActionResult();
        }

        [HttpPost("{posId}/notificationsStatus")]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public async Task<IActionResult> SetNotificationsStatusAsync(int posId,
            [FromBody] PosNotificationsStatusDto posNotificationsStatusDto)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(posId);
            if (pos == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о витрине не найдена" });

            if (posNotificationsStatusDto.AreNotificationsEnabled)
            {
                pos.EnableNotifications();
            }
            else
            {
                pos.DisableNotifications();
            }

            await UnitOfWork.CompleteAsync();

            return Ok();
        }


        [HttpGet("version")]
        [Permission(nameof(PosVersionManagementPermission))]
        public async Task<IActionResult> GetVersionAsync()
        {
            var configurationKeyValueResult =
                await _configurationManager.TryGetValueByKeyAsync(ConfigurationKeyIdentifier
                    .PointsOfSaleRequiredMinVersion);

            if (!configurationKeyValueResult.Succeeded)
                return NotFound(new ErrorResponseDto { Error = "Запись конфигурации не найдена" });

            return Ok(new PosVersionDto(configurationKeyValueResult.Value));
        }

        [HttpPut("version")]
        [Permission(nameof(PosVersionManagementPermission))]
        public async Task<IActionResult> UpdateVersionAsync([FromBody] PosVersionDto versionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var value = versionDto.Value;
            var settingValueResult =
                await _configurationManager.TrySetValueAsync(ConfigurationKeyIdentifier.PointsOfSaleRequiredMinVersion,
                    value);

            return settingValueResult.Succeeded
                ? (IActionResult)Ok()
                : BadRequest(settingValueResult.Error);
        }

        [HttpGet("{posId}/{measurementDateTimeFrom}/{measurementDateTimeTo}/posTemperatureChartRender")]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public IActionResult GetPosTemperatureChartForDateTimeRange(int posId, string measurementDateTimeFrom, string measurementDateTimeTo)
        {
            if (!DateTime.TryParse(measurementDateTimeFrom, out DateTime measurementPeriodStart))
                return BadRequest(
                    new ErrorResponseDto { Error = "Не указано корректное значение даты начала измерений" });

            if (!DateTime.TryParse(measurementDateTimeTo, out DateTime measurementPeriodEnd))
                return BadRequest(
                    new ErrorResponseDto { Error = "Не указано корректное значение даты окончания измерений" });

            measurementPeriodStart = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(measurementPeriodStart);
            measurementPeriodEnd = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(measurementPeriodEnd);
            var measurementsDateTimeRange = DateTimeRange.From(measurementPeriodStart, measurementPeriodEnd);
            var posStateWithinPeriod = _posEquipmentStateManager.GetPosStateWithinPeriod(posId, measurementsDateTimeRange);
            TempData["LastMeasurementDate"] = posStateWithinPeriod.Last().MeasurementDateTime;
            var chartRenderingViewModel = Mapper.Map<IEnumerable<PosEquipmentStateChartRenderingViewModel>>(posStateWithinPeriod);
            return Ok(chartRenderingViewModel);
        }

        [HttpGet("{posId}/posTemperatureChartRefresh")]
        public IActionResult GetLatestTemperature(int posId)
        {
            var dateOfLastMeasurement = (DateTime)TempData["LastMeasurementDate"];
            var currentMeasurementDate = dateOfLastMeasurement.Add(_chartSettings.ChartRefreshFrequency);
            TempData["LastMeasurementDate"] = currentMeasurementDate;
            var actualPosState = _posEquipmentStateManager.GetPosStateActualOnDate(posId, currentMeasurementDate);
            var chartRenderingViewModel = Mapper.Map<PosEquipmentStateChartRenderingViewModel>(actualPosState);

            chartRenderingViewModel.SetMeasurementPeriod(currentMeasurementDate, _chartSettings);

            return Ok(chartRenderingViewModel);
        }

        [HttpPost("{posId}/deactivatedPosWsMessages")]
        public async Task<IActionResult> GetLastReceivedMessageForDeactivatedPos(int posId)
        {
            var lastMessage = await _nasladdinApiClient.GetPosLastReceivedWsMessageAsync(posId);
            return Ok(lastMessage);
        }
    }
}