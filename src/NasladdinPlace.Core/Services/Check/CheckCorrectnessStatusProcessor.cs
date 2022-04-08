using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Printers.Localization;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Check
{
    public class CheckCorrectnessStatusProcessor : ICheckCorrectnessStatusProcessor
    {
        private readonly ILocalizedPrintersFactory<SimpleCheck> _checkPrinterFactory;
        private readonly IPrintedCheckLinkFormatter _printedCheckLinkFormatter;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISimpleCheckMaker _simpleCheckMaker;

        public CheckCorrectnessStatusProcessor(IServiceProvider serviceProvider)
        {
            _checkPrinterFactory = serviceProvider.GetRequiredService<ILocalizedPrintersFactory<SimpleCheck>>();
            _printedCheckLinkFormatter = serviceProvider.GetRequiredService<IPrintedCheckLinkFormatter>();
            _telegramChannelMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _simpleCheckMaker = serviceProvider.GetRequiredService<ISimpleCheckMaker>();
        }

        public Task<Result> RecheckLastPurchaseForUserAsync(int userId)
        {
            return ProcessAuditHistoryAndCorrectnessStatusForPosOperationAsync(
                unitOfWork => unitOfWork.PosOperations.GetUserLatestIncludingCheckItemsAsync(userId),
                CheckCorrectnessStatus.Incorrect,
                userId);
        }

        public Task<Result> ProcessCorrectnessStatusForPosOperationAsync(
            int posOperationId, 
            CheckCorrectnessStatus correctnessStatus,
            int userId)
        {
            return ProcessAuditHistoryAndCorrectnessStatusForPosOperationAsync(
                unitOfWork => unitOfWork.PosOperations.GetIncludingCheckItemsAsync(posOperationId),
                correctnessStatus,
                userId);
        }

        private async Task<Result> ProcessAuditHistoryAndCorrectnessStatusForPosOperationAsync(
            Func<IUnitOfWork, Task<PosOperation>> getPosOperationAsync, 
            CheckCorrectnessStatus correctnessStatus,
            int userId)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var posOperation = await getPosOperationAsync(unitOfWork);

                    if (posOperation == null)
                        return Result.Failure("Latest PosOperation is not found.");

                    if (userId != posOperation.UserId)
                        return Result.Failure("PosOperation does not belong to User.");

                    if (posOperation.CorrectnessStatus == correctnessStatus || correctnessStatus == CheckCorrectnessStatus.NotChecked)
                        return Result.Success();
                
                    switch (correctnessStatus)
                    {
                        case CheckCorrectnessStatus.Correct:
                            await SetAuditRequestDateTimeAndCorrectnessStatusForPosOperation(unitOfWork, posOperation,
                                correctnessStatus);
                            return Result.Success();
                        case CheckCorrectnessStatus.Incorrect:

                            var check = _simpleCheckMaker.MakeCheck(posOperation);

                            await SetAuditRequestDateTimeAndCorrectnessStatusForPosOperation(
                                unitOfWork,
                                posOperation,
                                correctnessStatus);

                            if (check.Summary.CostSummary.IsEmpty)
                                return Result.Success();

                            var message = GetTelegramMessage(posOperation.User.UserName, check);

                            await _telegramChannelMessageSender.SendAsync(message);

                            return Result.Success();
                        default:
                            return Result.Failure("Incorrect correctness status is found");
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    $"An error occured while updating correctness status in PosOperation. Exception: {ex}");
            }
        }

        private string GetTelegramMessage(string userName, SimpleCheck check)
        {
            var checkPrinter = _checkPrinterFactory.CreatePrinter(Language.English);

            var printedCheck = checkPrinter.Print(check);

            var printedCheckWithLink = _printedCheckLinkFormatter.ApplyFormat(printedCheck, check.Id);

            var message =
                $"{Emoji.ShoppingBags}Пользователь {userName} отправил запрос на проверку неправильного чека " +
                $"в витрине {check.OriginInfo.PosName}:{Environment.NewLine}" +
                $"{printedCheckWithLink}";
            return message;
        }

        private async Task SetAuditRequestDateTimeAndCorrectnessStatusForPosOperation(
            IUnitOfWork unitOfWork, 
            PosOperation posOperation, 
            CheckCorrectnessStatus correctnessStatus)
        {
            posOperation.SetCorrectnessStatus(correctnessStatus);
            posOperation.MarkAuditRequested();

            await unitOfWork.CompleteAsync();
        }
    }
}
