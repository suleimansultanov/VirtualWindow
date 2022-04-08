using System.Text;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.Purchase.Completion.Printer
{
    public class PurchaseCompletionResultRussianPrinter : IPurchaseCompletionResultPrinter
    {
        private readonly IPrintedCheckLinkFormatter _printedCheckLinkStringFormatter;

        public PurchaseCompletionResultRussianPrinter(IPrintedCheckLinkFormatter printedCheckLinkStringFormatter)
        {
            _printedCheckLinkStringFormatter = printedCheckLinkStringFormatter;
        }

        public string Print(PurchaseCompletionResult purchaseCompletionResult, string printedCheck)
        {
            var purchaseCompletionMessageBuilder = new StringBuilder();

            var user = purchaseCompletionResult.User;

            if (purchaseCompletionResult.Status == PurchaseCompletionStatus.PaymentError)
            {
                purchaseCompletionMessageBuilder.AppendFormat(
                    "{0} Пользователь {1} не смог завершить покупку в витрине \"{2}\" из-за проблем с оплатой через эквайринг. Детали ошибки: {3}.\n {4}",
                    Emoji.ShoppingCart,
                    user.UserName,
                    purchaseCompletionResult.Check.OriginInfo.PosName,
                    purchaseCompletionResult.Error.LocalizedDescription,
                    AddLinkToPrintedCheck(printedCheck,
                        purchaseCompletionResult.Check.Id));
            }

            return purchaseCompletionMessageBuilder.ToString();
        }

        private string AddLinkToPrintedCheck(string printedMessage, int posOperationId)
        {
            return _printedCheckLinkStringFormatter.ApplyFormat(printedMessage, posOperationId);
        }
    }
}
