using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseCompletion
{
    public class PurchaseCompletionDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PurchaseCompletionDiagnosticsStepCleaner(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task CleanUpAsync(DiagnosticsContext context)
        {
            if (context.PosOperation == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseCompletionDiagnosticsStepCleaner)} must have a pos operation.");

            var posId = context.PosOperation.PosId;
            var userId = context.PosOperation.UserId;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperations =
                    await unitOfWork.PosOperations.GetDetailedByPosAndUserAsync(posId, userId);

                var checkItems = await DeleteCheckItemsAsync(unitOfWork, posOperations);
                await ResetLabeledGoodsAsync(unitOfWork, checkItems);
                await DeleteBankTransactionInfosAsync(unitOfWork, posOperations);
                await DeleteFiscalizationInfosAsync(unitOfWork, posOperations);
                await DeletePosDoorsStatesAsync(unitOfWork, posOperations);
                await DeletePosOperationsAsync(unitOfWork, posOperations);
            }
        }

        private async Task<IEnumerable<CheckItem>> DeleteCheckItemsAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            var checkItems = posOperations.SelectMany(po => po.CheckItems).ToImmutableList();
            
            foreach (var checkItem in checkItems)
            {
                unitOfWork.CheckItems.Remove(checkItem.Id);
            }
            await unitOfWork.CompleteAsync();

            return checkItems;
        }

        private async Task ResetLabeledGoodsAsync(IUnitOfWork unitOfWork, IEnumerable<CheckItem> checkItems)
        {
            var labeledGoods = checkItems.Select(ci => ci.LabeledGood);
            foreach (var labeledGood in labeledGoods)
            {
                labeledGood.MarkAsNotBelongingToUserOrPos();
            }
            await unitOfWork.CompleteAsync();
        }

        private async Task DeleteBankTransactionInfosAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            var bankTransactionInfos = posOperations.SelectMany(po => po.BankTransactionInfos);
            var bankTransactionInfoRepository = unitOfWork.GetRepository<BankTransactionInfo>();
            foreach (var bankTransactionInfo in bankTransactionInfos)
            {
                bankTransactionInfoRepository.Remove(bankTransactionInfo.Id);
            }
            await unitOfWork.CompleteAsync();
        }

        private async Task DeleteFiscalizationInfosAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            var fiscalizationInfos = posOperations.SelectMany(po => po.FiscalizationInfos);
            var fiscalizationInfosRepository = unitOfWork.GetRepository<FiscalizationInfo>();
            foreach (var fiscalizationInfo in fiscalizationInfos)
            {
                fiscalizationInfosRepository.Remove(fiscalizationInfo.Id);
            }
            await unitOfWork.CompleteAsync();
        }

        private async Task DeletePosDoorsStatesAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            var doorsStates = posOperations.SelectMany(po => po.PosDoorsStates);
            unitOfWork.PosDoorsStates.RemoveRange(doorsStates);
            await unitOfWork.CompleteAsync();
        }

        private async Task DeletePosOperationsAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            foreach (var posOperation in posOperations)
            {
                unitOfWork.PosOperations.Remove(posOperation);
            }
            await unitOfWork.CompleteAsync();
        }
        
    }
}