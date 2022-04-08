using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.EmailSender.Contracts;
using NasladdinPlace.Api.Services.EmailSender.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.OverdueGoods.Checker;
using NasladdinPlace.Core.Services.OverdueGoods.Makers;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Converter;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Mapper;
using NasladdinPlace.Core.Services.OverdueGoods.Printer;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Api.Services.OverdueGoods
{
    public static class OverdueGoodsCheckerExtensions
    {
        public static void AddOverdueGoodsChecker(this IServiceCollection services, IConfigurationReader configurationReader)
        {
            services.AddSingleton<IGoodInstanceMapper, MoscowGoodInstanceMapper>();
            services.AddSingleton<IGoodInstancesByPosGrouper, GoodInstancesByPosGrouper>();
            services.AddSingleton<IOverdueGoodsInfoMaker, OverdueGoodsInfoMaker>();
            services.AddSingleton<IOverdueGoodsChecker, OverdueGoodsChecker>();
            services.AddSingleton<IOrderedObjectStringFormatter<GoodInstance>, OverdueGoodStringFormatter>();

            var adminPageBaseUrl = configurationReader.GetAdminPageBaseUrl();
            var overdueGoodsAdminPageBaseUrl = configurationReader.GetOverdueGoodsAdminPageBaseUrl();
            var fullUrl = ConfigurationReaderExt.CombineUrlParts( adminPageBaseUrl, overdueGoodsAdminPageBaseUrl );

             services.AddSingleton<IOrderedObjectStringFormatter<OverdueTypePosGoodInstances>>(sp =>
                new PosGoodInstancesStringFormatter(fullUrl));
            services.AddSingleton<IOrderedObjectStringFormatter<OverdueTypePosGoodInstances>>(sp =>
                new PosGoodInstancesStringFormatter(fullUrl));
            services.AddSingleton<IGoodInstancesGroupedByPointsOfSalePrinter, GoodInstancesGroupedByPointsOfSaleRussianPrinter>();
            services.AddSingleton<IOverdueGoodsInfoPrinter, OverdueGoodsInfoRussianPrinter>();
        }

        public static void UseOverdueGoodsChecker(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var overdueGoodsChecker = serviceProvider.GetRequiredService<IOverdueGoodsChecker>();

            overdueGoodsChecker.OnFoundOverdueGoods += (sender, groupedOverdueGoodsInfo) =>
            {
                var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
                using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
                {
                    if (!groupedOverdueGoodsInfo.TryGetValue(OverdueType.Overdue, out var overdueGoodsPosGroup))
                        return;

                    foreach (var goodInstance in overdueGoodsPosGroup.ToImmutableList())
                    {
                        var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(goodInstance.PosId);
                        posRealTimeInfo.OverdueGoodsNumber = goodInstance.OverdueGoods.Count;
                        unitOfWork.CompleteAsync();
                    }
                }
            };
        }

        public static void UseOverdueGoodsEmailNotifications(
            this IApplicationBuilder app,
            string subject,
            ICollection<string> destinationEmails)
        {
            var serviceProvider = app.ApplicationServices;

            var overdueGoodsChecker = serviceProvider.GetRequiredService<IOverdueGoodsChecker>();

            overdueGoodsChecker.OnFoundOverdueGoods += (sender, overdueGoodsInfo) =>
            {
                var emailSender = serviceProvider.GetRequiredService<IEmailSender>();
                var overdueGoodsInfoPrinter = serviceProvider.GetRequiredService<IOverdueGoodsInfoPrinter>();

                var emailMessageText = overdueGoodsInfoPrinter.Print(overdueGoodsInfo).ToHtml();

                var emailMessages = destinationEmails
                    .Select(destinationEmail => new TextEmailMessage(
                        destinationEmail, subject, emailMessageText, true))
                    .ToImmutableList();

                emailMessages.ForEach(message =>
                {
                    emailSender.SendAsync(message);
                });
            };
        }

        public static void UseOverdueGoodsTelegramNotifications(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var overdueGoodsChecker = serviceProvider.GetRequiredService<IOverdueGoodsChecker>();

            overdueGoodsChecker.OnFoundOverdueGoods += (sender, overdueGoodsInfo) =>
            {
                var telegramChannelMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
                var overdueGoodsInfoPrinter = serviceProvider.GetRequiredService<IOverdueGoodsInfoPrinter>();
                var message = overdueGoodsInfoPrinter.Print(overdueGoodsInfo);
                telegramChannelMessageSender.SendAsync(message);
            };
        }
    }
}