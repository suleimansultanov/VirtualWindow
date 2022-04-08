using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Interfaces;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;

namespace NasladdinPlace.Core.Models
{
    public class PosOperationTransactionBuilder
    {
        private readonly PosOperationTransaction _posOperationTransaction;
        private readonly Dictionary<PosOperationTransactionType, PosOperationTransaction> _operationTransactionsByType;

        private PosOperationTransactionBuilder(IReadonlyPosOperation posOperation)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            _operationTransactionsByType = new Dictionary<PosOperationTransactionType, PosOperationTransaction>
            {
                {PosOperationTransactionType.RegularPurchase, PosOperationTransaction.ForPayment(posOperation) },
                {PosOperationTransactionType.Refund, PosOperationTransaction.ForRefund(posOperation) },
                {PosOperationTransactionType.Addition, PosOperationTransaction.ForAddition(posOperation) },
                {PosOperationTransactionType.Verification, PosOperationTransaction.ForVerification(posOperation) }
            };
        }
        public PosOperationTransactionBuilder(
            IReadonlyPosOperation posOperation,
            PosOperationTransactionType transactionType): this(posOperation)
        {
            if (!Enum.IsDefined(typeof(PosOperationTransactionType), transactionType))
                throw new ArgumentException(nameof(transactionType));

            if (!_operationTransactionsByType.ContainsKey(transactionType))
                throw new ArgumentOutOfRangeException(nameof(transactionType), $"Incorrect operation transaciton type - {transactionType.ToString().ToLower()}");

            _posOperationTransaction = _operationTransactionsByType[transactionType];
        }

        public PosOperationTransactionBuilder MarkAsInProcess()
        {
            _posOperationTransaction.MarkAsInProcess();

            return this;
        }

        public PosOperationTransactionBuilder CalculateAndSetAmounts(IReadOnlyCollection<IReadonlyCheckItem> checkItems, decimal bonusPoints)
        {
            _posOperationTransaction.CalculateAndSetAmounts(checkItems, bonusPoints);

            return this;
        }

        public PosOperationTransactionBuilder AddTransactionCheckItems(
            IPosOperationTransactionCheckItemsMaker transactionCheckItemsMaker,
            IReadOnlyList<IReadonlyCheckItem> checkItems, 
            decimal bonusPoints)
        {
            _posOperationTransaction.AddTransactionCheckItems(transactionCheckItemsMaker, checkItems, bonusPoints);

            return this;
        }

        public PosOperationTransaction Build()
        {
            return _posOperationTransaction;
        }
    }
}
