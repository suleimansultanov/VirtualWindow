using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Services.PromotionNotifications;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionAgent;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class JobsSchedulerController : Controller
    {
        private readonly ITasksAgent _tasksAgent;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPromotionAgent _promotionAgent;
        private readonly IServiceProvider _serviceProvider;

        public JobsSchedulerController(ITasksAgent tasksAgent, 
                                       IUnitOfWorkFactory unitOfWorkFactory,
                                       IPromotionAgent promotionAgent,
                                       IServiceProvider serviceProvider)
        {
            _tasksAgent = tasksAgent;
            _unitOfWorkFactory = unitOfWorkFactory;
            _promotionAgent = promotionAgent;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult GetAllJobs()
        {
            var schedules = _tasksAgent.GetSchedules().Select(s => new ScheduleDto
            {
                Name = s.Name,
                NextRun = s.NextRun,
                Disabled = s.Disabled
            });

            return Ok(schedules);
        }

        [HttpPost("promotionAgentRestart")]
        public async Task<IActionResult> RestartPromotionJob([FromBody] PromotionDto promotionDto)
        {
            if (!Enum.IsDefined(typeof(PromotionType), promotionDto.PromotionType))
                return BadRequest("Невозможно определить тип промо акции");

            var promotionTypeEnum = (PromotionType)promotionDto.PromotionType;
            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var promotionSetting = await unitOfWork.PromotionSettings.GetByPromotionTypeAsync(promotionTypeEnum);
                if (promotionSetting == null)
                    return BadRequest("Настройки для указанного типа промо акции не заданы");

                if (!(promotionSetting.IsEnabled && promotionSetting.IsNotificationEnabled))
                    return BadRequest("Промо акция отключена и не может быть перезапущена");

                _promotionAgent.Stop(promotionTypeEnum);
                PromotionNotificationsExtensions.AddPromotionNotificationsAgent(
                    _serviceProvider, promotionType: promotionSetting.PromotionType,
                    promotionTime: promotionSetting.NotificationStartTime
                );

                return Ok((byte)promotionTypeEnum);
            }            
        }

        [HttpDelete("promotionAgentStop/{promotionType}")]
        public async Task<IActionResult> StopPromotionJob(byte promotionType)
        {
            if (!Enum.IsDefined(typeof(PromotionType), promotionType))
                return BadRequest("Невозможно определить тип промо акции");

            var promotionTypeEnum = (PromotionType)promotionType;
            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var promotionSetting = await unitOfWork.PromotionSettings.GetByPromotionTypeAsync(promotionTypeEnum);
                if (promotionSetting == null)
                    return BadRequest("Настройки для указанного типа промо акции не заданы");

                if (promotionSetting.IsEnabled && promotionSetting.IsNotificationEnabled)
                    return BadRequest("Промо акция включена и не может быть остановлена");

                _promotionAgent.Stop(promotionTypeEnum);

                return Ok((byte)promotionTypeEnum);
            }
        }
    }
}