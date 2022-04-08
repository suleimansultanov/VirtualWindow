using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureAgent;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Pos.Temperature.Extensions
{
    public static class PosTemperatureNotificationsExtensions
    {       
        public static void UsePosTemperatureNotifications(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
            var services = app.ApplicationServices;
            var posTemperatureAgent = services.GetRequiredService<IPosTemperatureAgent>();
            var unitOfWorkFactory = services.GetRequiredService<IUnitOfWorkFactory>();
            var telegramChannelMessageSender = services.GetRequiredService<ITelegramChannelMessageSender>();

            var adminPageBaseUrl = configurationReader.GetAdminPageBaseUrl();

            posTemperatureAgent.OnAbnormalTemperatureDetected += (sender, abnormalPosesTemperatures) =>
            {
                Task.Run(() => SendNotificationAsync(abnormalPosesTemperatures, adminPageBaseUrl, unitOfWorkFactory, telegramChannelMessageSender));
            };

            posTemperatureAgent.OnPosNoTemperatureUpdateDetected += (sender, posIds) =>
            {
                Task.Run(() => PerformNoTemperatureUpdatesNotification(posIds, adminPageBaseUrl, unitOfWorkFactory, telegramChannelMessageSender));
            };

            var posTemperatureStateCheckIntervalInMinutes = configurationReader.GetPosTemperatureStateCheckIntervalInMinutes();
            var tasksAgentOptions = TasksAgentOptions.FixedPeriodOfTime(TimeSpan.FromMinutes(posTemperatureStateCheckIntervalInMinutes));
            posTemperatureAgent.Start(tasksAgentOptions);
        }

        private static async Task SendNotificationAsync(
            IEnumerable<PosTemperature> abnormalPosesTemperatures,
            string adminPageBaseUrl,
            IUnitOfWorkFactory unitOfWorkFactory,
            ITelegramChannelMessageSender telegramChannelMessageSender)
        {
            var telegramMessageBuilder = new StringBuilder();

            using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
            {
                foreach (var averageTemperature in abnormalPosesTemperatures)
                {
                    var pos = await unitOfWork.PointsOfSale.GetByIdAsync(averageTemperature.PosId);

                    if (AreNotificationsDisabledForAbnormalTemperaturesData(pos))
                        continue;

                    var link = GetPosLink(adminPageBaseUrl, averageTemperature.PosId);
                    telegramMessageBuilder.Append(
                        $"{Emoji.Thermometer} [{pos.AbbreviatedName}: {averageTemperature.Temperature:F1} °C.]({link})");
                    telegramMessageBuilder.Append(Environment.NewLine);
                }
            }

            if (telegramMessageBuilder.Length == 0)
                return;

            await telegramChannelMessageSender.SendAsync(telegramMessageBuilder.ToString());
        }

        private static async Task PerformNoTemperatureUpdatesNotification(
            IEnumerable<int> posIds,
            string adminPageBaseUrl,
            IUnitOfWorkFactory unitOfWorkFactory,
            ITelegramChannelMessageSender telegramChannelMessageSender)
        {
            var telegramMessageBuilder = new StringBuilder();
            using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
            {
                foreach (var posId in posIds)
                {
                    var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);

                    if (AreNotificationsDisabledForNoTemperatureData(pos))
                        continue;

                    var link = GetPosLink(adminPageBaseUrl, posId);
                    telegramMessageBuilder.Append(
                        $"{Emoji.Thermometer} [{pos.AbbreviatedName}: Нет данных по температуре]({link})");
                    telegramMessageBuilder.Append(Environment.NewLine);
                }
            }

            if (telegramMessageBuilder.Length == 0)
                return;

            await telegramChannelMessageSender.SendAsync(telegramMessageBuilder.ToString());
        }

        private static bool AreNotificationsDisabledForNoTemperatureData(Core.Models.Pos pos)
        {
            return !pos.AreNotificationsEnabled || !pos.IsInServiceOrInTestMode;
        }

        private static bool AreNotificationsDisabledForAbnormalTemperaturesData(Core.Models.Pos pos)
        {
            return !pos.AreNotificationsEnabled || !pos.IsInServiceMode;
        }

        private static string GetPosLink(string adminPageBaseUrl, int posId)
        {
           return $"{adminPageBaseUrl}/PointsOfSale/{posId}/Monitoring";
        }
    }
}