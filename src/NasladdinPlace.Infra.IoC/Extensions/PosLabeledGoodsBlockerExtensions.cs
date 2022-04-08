using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class PosLabeledGoodsBlockerExtensions
    {
        public static void AddPosLabeledGoodsBlocker(this IServiceCollection services)
        {
            services.AddSingleton<IPosLabeledGoodsBlocker, PosLabeledGoodsBlocker>();
            services.AddSingleton<ILabeledGoodsPrinter, LabeledGoodsPrinter>();
        }

        public static void UsePosLabeledGoodsBlockerTelegramNotifications(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            var labeledGoodsBlocker = services.GetRequiredService<IPosLabeledGoodsBlocker>();
            var labeledGoodsPrinter = services.GetRequiredService<ILabeledGoodsPrinter>();
            var telegramMessageSender = services.GetRequiredService<ITelegramChannelMessageSender>();

            labeledGoodsBlocker.LabeledGoodsBlocked += (sender, posLabeledGoods) =>
            {
                var message = labeledGoodsPrinter.Print(
                    $"{Emoji.Japanese_Goblin} Заблокированные RFID метки витрины {posLabeledGoods.PosName}:",
                    posLabeledGoods.LabeledGoods
                );
                telegramMessageSender.SendAsync(message);
            };
        }
    }

}
