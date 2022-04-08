using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Reports;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.Logging;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    public class ReportsUploadingInfosController : BaseController
    {
        private readonly ISpreadsheetProvider _spreadsheetProvider;
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsUploadingInfosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _spreadsheetProvider = serviceProvider.GetRequiredService<ISpreadsheetProvider>();
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        [HttpPut]
        [Permission(nameof(ReportCrudPermission))]
        public async Task<IActionResult> EditReportsUploadingInfoAsync(
            [FromBody] ReportUploadingInfoViewModel viewModel)
        {
            var spreadsheet = _spreadsheetProvider.Provide(viewModel.Url);

            try
            {
                await spreadsheet.IsAllowedToEditAsync();
            }
            catch (GoogleApiException e)
            {
                switch (e.HttpStatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return NotFound(new ErrorResponseDto { Error = "Таблица не найдена" });
                    case HttpStatusCode.Forbidden:
                        return BadRequest(new ErrorResponseDto { Error = "Нет прав на редактирование таблицы" });
                }

                return BadRequest(new ErrorResponseDto { Error = "Неверный URL таблицы" });
            }

            var uniqueType =
                await UnitOfWork.ReportsUploadingInfo.GetReportsUploadingInfoByType((ReportType)viewModel.Type);

            if (!(uniqueType == null || uniqueType.Id == viewModel.Id))
                return BadRequest(new ErrorResponseDto { Error = "Неизвестный тип отчета" });

            var reportUploadingInfo = UnitOfWork.ReportsUploadingInfo.GetById(viewModel.Id);

            reportUploadingInfo.Update(viewModel.Url, (ReportType)viewModel.Type, viewModel.Description,
                viewModel.Sheet, viewModel.BatchSize);

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost]
        [Permission(nameof(ReportCrudPermission))]
        public async Task<IActionResult> AddReportsUploadingInfoAsync([FromBody] ReportUploadingInfoViewModel viewModel)
        {
            var uniqueType =
                await UnitOfWork.ReportsUploadingInfo.GetReportsUploadingInfoByType((ReportType)viewModel.Type);

            if (uniqueType != null)
                return BadRequest(new ErrorResponseDto { Error = "Информация о выгрузке отчета уже существует" });

            UnitOfWork.ReportsUploadingInfo.Add(new ReportUploadingInfo(viewModel.Url, (ReportType)viewModel.Type,
                viewModel.Description, viewModel.Sheet, viewModel.BatchSize));

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("{type}")]
        [Permission(nameof(ReportUploadingPermission))]
        public async Task<IActionResult> UploadAsync(int type)
        {
            try
            {
                var uploadReportResult = await _nasladdinApiClient.UploadReportAsync(type);

                var userId = GetUserIdAsInt(User);

                if (uploadReportResult.Status != ResultStatus.Success)
                {
                    _logger.LogError($"User {userId} has tried upload report with type {type}, but was error {uploadReportResult.Error}");
                    return BadRequest(uploadReportResult.Error);
                }

                _logger.LogInfo($"User {userId} has uploaded successful report with type {type}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occurred during upload a report with type {type}. Verbose error: {ex}");
                return BadRequest("Произошла ошибка при выгрузке отчёта. Пожалуйста, обратитесь к администратору.");
            }
        }

        [HttpDelete]
        [Permission(nameof(ReportCrudPermission))]
        public async Task<IActionResult> DeleteReportsUploadingInfoAsync(
            [FromBody] ReportUploadingInfoViewModel viewModel)
        {
            var reportUploadingInfo = UnitOfWork.ReportsUploadingInfo.GetById(viewModel.Id);

            if (reportUploadingInfo == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о выгрузке отчета не найдена" });

            UnitOfWork.ReportsUploadingInfo.Remove(reportUploadingInfo.Id);
            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        private int GetUserIdAsInt(ClaimsPrincipal principal)
        {
            return int.Parse(_userManager.GetUserId(principal));
        }
    }
}