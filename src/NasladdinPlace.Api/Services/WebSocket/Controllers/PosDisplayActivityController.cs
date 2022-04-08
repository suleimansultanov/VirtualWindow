using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.PosDisplay;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;
using NasladdinPlace.Core.Services.PosScreenResolution.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class PosDisplayActivityController : WsController
    {
        private const string WindowsOperatingSystem = "Windows";
        private readonly IPosComponentsActivityManager _posComponentsActivityManager;
        private readonly IIdFromGroupFetcher _idFromGroupFetcher;
        private readonly IPosDisplayCommandsManager _posDisplayCommandsManager;
        private readonly IPosScreenResolutionUpdater _posScreenResolutionUpdater;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosDisplayActivityController(IServiceProvider serviceProvider)
        {
            _posComponentsActivityManager = serviceProvider.GetRequiredService<IPosComponentsActivityManager>();
            _idFromGroupFetcher = serviceProvider.GetRequiredService<IIdFromGroupFetcher>();
            _posDisplayCommandsManager = serviceProvider.GetRequiredService<IPosDisplayCommandsManager>();
            _posScreenResolutionUpdater = serviceProvider.GetRequiredService<IPosScreenResolutionUpdater>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        }

        [Obsolete("Will be removed in future release.")]
        public async Task NotifyPosDisplayActivity(PosDisplayActvityInfo groupInfo)
        {
            var posDisplayId = _idFromGroupFetcher.Fetch(groupInfo.Group, Groups.PosDisplay);

            if (posDisplayId == 0) return;

            _posComponentsActivityManager.PosDisplays.AddOrUpdate(posDisplayId);

            if (groupInfo.ScreenResolution == null) return;

            var screenResolution = new ScreenResolution(
                groupInfo.ScreenResolution.Width,
                groupInfo.ScreenResolution.Height
            );
            await _posScreenResolutionUpdater.UpdateAsync(posDisplayId, screenResolution);
        }

        [Obsolete("Will be removed in future release.")]
        public async Task NotifyPosBatteryActivity(PosActivityInfo groupInfo)
        {
            await NotifyPosActivity(groupInfo);
        }

        public async Task NotifyPosActivity(PosActivityInfo groupInfo)
        {
            var posDisplayId = _idFromGroupFetcher.Fetch(groupInfo.Group, Groups.PosDisplay);

            if (posDisplayId == 0 || groupInfo.ScreenResolution == null) return;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                _posComponentsActivityManager.PosDisplays.AddOrUpdate(posDisplayId);

                var battery = GetBatteryInfo(groupInfo);

                var deviceInfo = new PosDeviceInfo();

                deviceInfo.SetUserAgent(groupInfo.UserAgent);

                var screenResolution = new ScreenResolution(
                        groupInfo.ScreenResolution.Width,
                        groupInfo.ScreenResolution.Height
                    );

                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posDisplayId);

                await _posScreenResolutionUpdater.UpdateAsync(unitOfWork, posDisplayId, screenResolution);

                var device = posRealTimeInfo.Devices.FirstOrDefault(d => d.UserAgent == groupInfo.UserAgent);

                if (device == null)
                {
                    deviceInfo.Update(screenResolution, battery);
                    
                    posRealTimeInfo.Devices.Add(deviceInfo);
                    _posComponentsActivityManager.PosBattery.AddOrUpdate(posDisplayId, deviceInfo);
                }
                else
                {
                    device.Update(screenResolution, battery);
                    _posComponentsActivityManager.PosBattery.AddOrUpdate(posDisplayId, device);
                }

                await unitOfWork.CompleteAsync();
            }
        }

        public Task ConfirmCommandDelivery(PosDisplayCommandDeliveryDto commandInfo)
        {
            _posDisplayCommandsManager.ConfirmPosDisplayCommandDelivered(commandInfo);

            return Task.CompletedTask;
        }

        private BatteryInfo GetBatteryInfo(PosActivityInfo groupInfo)
        {
            var isTablet = !groupInfo.UserAgent.Contains(WindowsOperatingSystem);

            var batteryLevel = groupInfo.BatteryLevel * 100;

            var batteryIsCharging = groupInfo.BatteryIsCharging;

            return new BatteryInfo(batteryLevel, batteryIsCharging, isTablet);
        }
    }
}