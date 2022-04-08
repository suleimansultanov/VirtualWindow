using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Shared;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Promotions;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(PromotionSettingsPermission))]
    public class PromotionSettingsController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public PromotionSettingsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpPut]
        public async Task<IActionResult> EditPromotionAsync([FromBody] PromotionSettingViewModel viewModel)
        {
            var promotionSettingsRepository = UnitOfWork.GetRepository<PromotionSetting>();

            var dbPromotionSetting = await promotionSettingsRepository.GetAll()
                .SingleOrDefaultAsync(p => p.Id == viewModel.PromotionSettingId);

            if (dbPromotionSetting == null)
                return NotFound(new ErrorResponseDto { Error = "Информация о промо акции не найдена" });

            FillDbModelFromViewModel(dbPromotionSetting, viewModel);
            promotionSettingsRepository.Update(dbPromotionSetting);

            await UnitOfWork.CompleteAsync();

            RequestResponse<byte> response;
            if (dbPromotionSetting.IsEnabled && dbPromotionSetting.IsNotificationEnabled)
            {
                response = await _nasladdinApiClient.RestartPromotionAgentAsync(new PromotionDto
                {
                    PromotionType = (byte)dbPromotionSetting.PromotionType
                });
                if (response.IsRequestSuccessful)
                    return Ok(new SuccessResponseDto { Message = "Служба планировщика задач успешно перезапущена" });

                return BadRequest(response.Error);
            }

            response = await _nasladdinApiClient.StopPromotionAgentAsync((byte)dbPromotionSetting.PromotionType);

            if (response.IsRequestSuccessful)
                return Ok(new SuccessResponseDto { Message = "Служба планировщика задач успешно остановлена" });

            return BadRequest(response.Error);
        }

        private static void FillDbModelFromViewModel(PromotionSetting dbPromotionSetting,
            PromotionSettingViewModel viewModel)
        {
            dbPromotionSetting.Update(viewModel.PromotionType, viewModel.BonusValue, viewModel.NotificationStartTime);

            if (viewModel.IsNotificationEnabled)
            {
                dbPromotionSetting.EnableNotification();
            }
            else
            {
                dbPromotionSetting.DisableNotification();
            }

            if (viewModel.IsEnabled)
            {
                dbPromotionSetting.Enable();
            }
            else
            {
                dbPromotionSetting.Disable();
            }
        }
    }
}