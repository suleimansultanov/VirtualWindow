using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Users.Bonus.Agent;
using NasladdinPlace.Core.Services.Users.Bonus.Agent.Contracts;
using NasladdinPlace.Core.Services.Users.Bonus.Manager;
using NasladdinPlace.Core.Services.Users.Bonus.Manager.Contracts;
using NasladdinPlace.Core.Services.Users.Bonus.Printer;
using NasladdinPlace.Core.Services.Users.Bonus.Printer.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.UsersBonusPoints
{
    public static class UsersBonusExtensions
    {
        public static void AddAbnormalUserBonusPointsSeekingAgent(this IServiceCollection services,
            decimal maxUserBonusPointsAmount)
        {
            services.AddSingleton<IAbnormalUsersBonusPointsAlertPrinter, AbnormalUsersBonusPointsAlertRussianPrinter>();
            services.AddSingleton<IAbnormalUserBonusPointsSeekingManager>(sp =>
                new AbnormalUserBonusPointsSeekingManager(sp.GetRequiredService<IUnitOfWorkFactory>(), maxUserBonusPointsAmount));
            services.AddSingleton<IAbnormalUserBonusPointsSeekingAgent>(sp => new AbnormalUserBonusPointsSeekingAgent(
                sp.GetRequiredService<ITasksAgent>(),
                sp.GetRequiredService<IAbnormalUserBonusPointsSeekingManager>()
            ));
        }

        public static void UseAbnormalUsersBonusPointsTelegramAlerts(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var abnormalUserBonusPointsSeekingManager = serviceProvider.GetRequiredService<IAbnormalUserBonusPointsSeekingManager>();
            var telegramMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
            var abnormalUsersBonusPointsAlertPrinter = serviceProvider.GetRequiredService<IAbnormalUsersBonusPointsAlertPrinter>();

            abnormalUserBonusPointsSeekingManager.OnFoundUsersHavingAbnormalBonusPointsAmount += async (sender, usersHavingAbnormalBonus) =>
            {
                var message = abnormalUsersBonusPointsAlertPrinter.Print(usersHavingAbnormalBonus);
                await telegramMessageSender.SendAsync(message);
            };
        }

        public static void RunAbnormalUserBonusPointsSeekingAgent(this IApplicationBuilder app, TasksAgentOptions options)
        {
            var serviceProvider = app.ApplicationServices;
            var bonusUpperAgent = serviceProvider.GetRequiredService<IAbnormalUserBonusPointsSeekingAgent>();
            bonusUpperAgent.Start(options);
        }
    }
}