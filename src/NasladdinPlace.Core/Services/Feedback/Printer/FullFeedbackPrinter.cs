using System;
using System.Text;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Formatters;
using NasladdinPlace.Core.Services.Formatters.Contracts;
using NasladdinPlace.Core.Services.Printers.Localization;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Services.Feedback.Printer
{
    public class FullFeedbackPrinter : IFeedbackPrinter
    {
        private const string NoInformation = "No info";
        private const string NoPurchasesYet = "No purchases yet";

        private readonly StringBuilder _feedbackStringBuilder;
        private readonly ILocalizedPrintersFactory<SimpleCheck> _simpleCheckLocalizedPrintersFactory;
        private readonly IPrintedCheckLinkFormatter _printedCheckLinkFormatter;
        private readonly ILinkWrapper _telegramLinkWrapper;

        public FullFeedbackPrinter(ILocalizedPrintersFactory<SimpleCheck> simpleCheckLocalizedPrintersFactory, IPrintedCheckLinkFormatter printedCheckLinkFormatter,
            ILinkWrapper telegramLinkWrapper)
        {
            if (simpleCheckLocalizedPrintersFactory == null)
                throw new ArgumentNullException(nameof(simpleCheckLocalizedPrintersFactory));
            if (printedCheckLinkFormatter == null)
                throw new ArgumentNullException(nameof(printedCheckLinkFormatter));
            if (telegramLinkWrapper == null)
                throw new ArgumentNullException(nameof(telegramLinkWrapper));

            _simpleCheckLocalizedPrintersFactory = simpleCheckLocalizedPrintersFactory;
            _printedCheckLinkFormatter = printedCheckLinkFormatter;
            _telegramLinkWrapper = telegramLinkWrapper;
            _feedbackStringBuilder = new StringBuilder();
        }

        public string Print(Models.Feedback.Feedback feedback)
        {
            AppendContent(feedback);
            AppendSenderInformation(feedback);
            AppendPosInfo(feedback);
            AppendFeedbackCreationDateTime(feedback);
            AppendSenderLastPurchaseInfo(feedback);
            AppendSenderDeviceInfo(feedback);

            return _feedbackStringBuilder.ToString();
        }

        private void AppendContent(Models.Feedback.Feedback feedback)
        {
            AppendSection($"{Emoji.Loudspeaker} {feedback.Body.Content}{Environment.NewLine}");
        }

        private void AppendSenderInformation(Models.Feedback.Feedback feedback)
        {
            var senderInfo = feedback.SenderInfo;
            if (!senderInfo.IsSenderUnauthorized)
            {
                var link =
                    _telegramLinkWrapper.Wrap($"{senderInfo.User.UserName}/{senderInfo.User.Id}", LinkFormatType.UsersListPage,
                        senderInfo.User.Id);
                AppendSection("User Name/ID:", link);
            }

            AppendSenderPhoneNumber(feedback);
        }

        private void AppendSenderPhoneNumber(Models.Feedback.Feedback feedback)
        {
            string phoneNumber = feedback.SenderInfo.PhoneNumber;
            string userPhoneNumber = phoneNumber == string.Empty || phoneNumber == null ? NoInformation : $"+{phoneNumber}";
            AppendSection("Phone:", userPhoneNumber);
        }

        private void AppendPosInfo(Models.Feedback.Feedback feedback)
        {
            string content = NoInformation;
            if (feedback.SenderInfo.HasLastPurchaseInfo)
            {
                var pos = feedback.SenderInfo.LastPurchaseInfo.PosOperation.Pos;
                content = _telegramLinkWrapper.Wrap(pos.Name, LinkFormatType.PosDetailsPage, pos.Id);
            }
            AppendSection("POS:", content);
        }

        private void AppendFeedbackCreationDateTime(Models.Feedback.Feedback feedback)
        {
            AppendSection("Date:", ConvertToMoscowDateTimeString(feedback.DateCreated));
        }

        private void AppendSenderLastPurchaseInfo(Models.Feedback.Feedback feedback)
        {
            switch (feedback.SenderInfo.Status)
            {
                case SenderInfoStatus.Unauthorized:
                    AppendNoInfoAboutPurchases();
                    return;
                case SenderInfoStatus.NoPosOperations:
                    AppendSenderNoLastPurchasesInfo();
                    return;
                case SenderInfoStatus.HasLastPurchase:
                    AppendLastPurchaseFullInfo(feedback);
                    return;
            }
        }

        private void AppendSenderDeviceInfo(Models.Feedback.Feedback feedback)
        {
            var deviceInfo = feedback.SenderInfo.DeviceInfo;
            AppendSection("Phone model, OS:", $"{deviceInfo.DeviceName}, {deviceInfo.OperatingSystem}");
            AppendSection("App version:", feedback.AppInfo.AppVersion);
        }

        private void AppendLastPurchaseFullInfo(Models.Feedback.Feedback feedback)
        {
            if (feedback.SenderInfo.HasLastPurchaseInfo)
            {
                var check = feedback.SenderInfo.LastPurchaseInfo.LastSimpleCheck;
                if (check.IsEmpty)
                {
                    AppendNoInfoAboutPurchases();
                }
                else
                {
                    var checkPrinter = _simpleCheckLocalizedPrintersFactory.CreatePrinter(Language.English, false);
                    var printedCheck = checkPrinter.Print(check);
                    var printedCheckWithLink = _printedCheckLinkFormatter.ApplyFormat(printedCheck, check.Id);
                    _feedbackStringBuilder.AppendLine("*Last purchase:*");
                    _feedbackStringBuilder.AppendLine(printedCheckWithLink);
                }
                AppendSection("Purchase date:", ConvertToMoscowDateTimeString(check.DateCreated));
                AppendSection("User balance:", $"{feedback.SenderInfo.PaymentBalance.MoneySum.Value:0.00} руб");

                return;
            }

            AppendSenderNoLastPurchasesInfo();
        }

        private void AppendSenderNoLastPurchasesInfo()
        {
            AppendSection("Last purchase:", NoPurchasesYet);
        }

        private void AppendNoInfoAboutPurchases()
        {
            AppendSection("Last purchase:", NoInformation);
        }

        private void AppendSection(string header, string content = null)
        {
            _feedbackStringBuilder.AppendLine($"*{header}* {content}");
        }

        private string ConvertToMoscowDateTimeString(DateTime utcDateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(utcDateTime);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }
    }
}