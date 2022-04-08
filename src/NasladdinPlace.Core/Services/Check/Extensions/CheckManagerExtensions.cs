using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.Check.Extensions
{
    public static class CheckManagerExtensions
    {
        public static void UseDisablingRefundedCheckItems(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var checkManager = services.GetRequiredService<ICheckManager>();
            var unitOfWorkFactory = services.GetRequiredService<IUnitOfWorkFactory>();
            var posLabeledGoodsBlocker = services.GetRequiredService<IPosLabeledGoodsBlocker>();

            checkManager.CheckItemsDeletedOrRefunded += async (sender, checkItems) =>
            {
                var groupCheckItemsByPosId = checkItems.GroupBy(cki => cki.PosId).ToList();

                using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
                {
                    foreach (var g in groupCheckItemsByPosId)
                    {
                        var labels = g.Select(cki => cki.LabeledGood.Label);
                        var posContent = new PosContent(g.Key, labels);
                        await posLabeledGoodsBlocker.BlockAsync(unitOfWork, posContent);
                    }
                }
            };
        }

        public static void UseCheckManagerSmsUserNotifier(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var checkManager = services.GetRequiredService<ICheckManager>();
            var checkManagerSmsSender = services.GetRequiredService<ICheckManagerSmsSender>();

            checkManager.CheckItemsEditingCompleted += async (sender, checkEditingInfo) =>
            {
                await checkManagerSmsSender.SendSmsAsync(checkEditingInfo);
            };
        }

        public static void AddCheckManagerSmsSender( this IServiceCollection services,
	        IConfigurationReader configurationReader )
        {
	        var refundMessageFormat = configurationReader.GetRefundMessageFormat();
	        var isPermittedToNotifyAboutRefund = configurationReader.GetIsPermittedToNotifyAboutRefund();
	        var additionOrVerificationMessageFormat = configurationReader.GetAdditionOrVerificationMessageFormat();
	        var isPermittedToNotifyAboutAdditionOrVerification =
		        configurationReader.GetIsPermittedToNotifyAboutAdditionOrVerification();

	        var notificationMessages = new CheckEditingNotificationMessages(
		        refund: new NotificationMessage( refundMessageFormat, isPermittedToNotifyAboutRefund ),
		        additionOrVerification:
		        new NotificationMessage( additionOrVerificationMessageFormat,
			        isPermittedToNotifyAboutAdditionOrVerification ) );

	        AddCheckManagerSmsSender( services, notificationMessages );
        }

        public static void AddCheckManagerSmsSender(this IServiceCollection services, CheckEditingNotificationMessages notificationMessages)
        {
            services.AddSingleton<ICheckManagerSmsSender>(sp =>
            {
                var smsSender = sp.GetRequiredService<ISmsSender>();
                var logger = sp.GetRequiredService<ILogger>();
                return new CheckManagerSmsSender(smsSender, logger, notificationMessages);
            });
        }
    }
}
