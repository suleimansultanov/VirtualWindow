using System;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions
{
    public class PosOperationTransactionCreationUpdatingService : IPosOperationTransactionCreationUpdatingService
    {
        private readonly IPosOperationTransactionCheckItemsMaker _transactionCheckItemsMaker;
        public PosOperationTransactionCreationUpdatingService(IPosOperationTransactionCheckItemsMaker transactionCheckItemsMaker)
        {
            if (transactionCheckItemsMaker == null)
                throw new ArgumentNullException(nameof(transactionCheckItemsMaker));

            _transactionCheckItemsMaker = transactionCheckItemsMaker;
        }

        public PosOperationTransaction CreateTransaction(
            PosOperationTransactionCreationInfo transactionCreationInfo)
        {
            if (transactionCreationInfo == null)
                throw new ArgumentNullException(nameof(transactionCreationInfo));

            var posOperationTransactionBuilder = 
                new PosOperationTransactionBuilder(transactionCreationInfo.PosOperation, transactionCreationInfo.TransactionType);

            var posOperationTransaction = posOperationTransactionBuilder
                .CalculateAndSetAmounts(transactionCreationInfo.CheckItems, transactionCreationInfo.BonusPoints)
                .AddTransactionCheckItems(_transactionCheckItemsMaker, transactionCreationInfo.CheckItems, transactionCreationInfo.BonusPoints)
                .Build();

            return posOperationTransaction;
        }

        public PosOperationTransaction UpdateTransaction(PosOperationTransactionUpdatingInfo operationTransactionUpdatingInfo)
        {
            if (operationTransactionUpdatingInfo == null)
                throw new ArgumentNullException(nameof(operationTransactionUpdatingInfo));

            var operationTransaction = operationTransactionUpdatingInfo.OperationTransaction;

            var checkItemsForModification = operationTransactionUpdatingInfo.CheckItemsToUpdate.Select(cki => cki).ToImmutableList();
            var checkItemsInTransaction = operationTransaction.PosOperationTransactionCheckItems.Select(pcki => pcki.CheckItem)
                .ToImmutableList();

            var checkItemsToAddInTransactionCheckItems = checkItemsForModification.Except(checkItemsInTransaction).ToList();

            operationTransaction.CalculateAndSetAmounts(operationTransactionUpdatingInfo.CheckItemsToUpdate, operationTransactionUpdatingInfo.BonusPoints);
            operationTransaction.AddTransactionCheckItems(_transactionCheckItemsMaker, checkItemsToAddInTransactionCheckItems, operationTransactionUpdatingInfo.BonusPoints);

            return operationTransaction;
        }
    }
}
