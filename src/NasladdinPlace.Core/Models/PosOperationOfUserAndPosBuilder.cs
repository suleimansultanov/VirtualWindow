using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.OperationsManager;

namespace NasladdinPlace.Core.Models
{
    public class PosOperationOfUserAndPosBuilder
    {
        private readonly PosOperation _posOperation;
        public static readonly Brand DefaultBrand = Brand.Invalid;
        private const PosMode DefaultMode = PosMode.Purchase;

        public PosOperationOfUserAndPosBuilder(int userId, int posId)
        {
            _posOperation = PosOperation.OfUserAndPos(userId, posId);
            _posOperation.Brand = DefaultBrand;
            _posOperation.Mode = DefaultMode;
        }

        public PosOperationOfUserAndPosBuilder SetId(int id)
        {
            _posOperation.Id = id;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetUser(ApplicationUser user)
        {
            _posOperation.User = user;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetPos(Pos pos)
        {
            _posOperation.Pos = pos;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetBrand(Brand brand)
        {
            _posOperation.Brand = brand;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetMode(PosMode mode)
        {
            _posOperation.Mode = mode;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetDateStarted(DateTime dateStarted)
        {
            _posOperation.DateStarted = dateStarted;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetDateSentForVerification(DateTime dateStarted)
        {
            _posOperation.CompletionInitiationDate = dateStarted;

            return this;
        }

        public PosOperationOfUserAndPosBuilder SetCheckItems(ICollection<CheckItem> checkItems)
        {
            if (_posOperation.CheckItems != null)
                _posOperation.CheckItems = checkItems;

            return this;
        }

        public PosOperationOfUserAndPosBuilder WriteOffBonusPoints()
        {
            _posOperation.WriteOffBonusPoints();

            return this;
        }

        public PosOperationOfUserAndPosBuilder AddPosOperationTransaction(PosOperationTransactionType transactionType = PosOperationTransactionType.RegularPurchase)
        {
            _posOperation.AddTransaction(transactionType);

            return this;
        }

        public PosOperationOfUserAndPosBuilder MarkAsCompleted()
        {
            MarkPosOperationAs(PosOperationStatus.Completed);

            return this;
        }

        public PosOperationOfUserAndPosBuilder MarkAsPendingCompletion()
        {
            MarkPosOperationAs(PosOperationStatus.PendingCompletion);

            return this;
        }

        public PosOperationOfUserAndPosBuilder MarkAsPendingPayment()
        {
            MarkPosOperationAs(PosOperationStatus.PendingPayment);

            return this;
        }

        public PosOperationOfUserAndPosBuilder MarkAsPaid(OperationPaymentInfo operationPaymentInfo)
        {
            _posOperation.MarkAsPaid(operationPaymentInfo);

            return this;
        }

        private void MarkPosOperationAs(PosOperationStatus status)
        {
            switch (status)
            {
                case PosOperationStatus.PendingCompletion:
                    _posOperation.MarkAsPendingCompletion();
                    break;
                case PosOperationStatus.PendingCheckCreation:
                    MarkPosOperationAs(PosOperationStatus.PendingCompletion);
                    _posOperation.MarkAsPendingCheckCreation();
                    break;
                case PosOperationStatus.Completed:
                    MarkPosOperationAs(PosOperationStatus.PendingCheckCreation);
                    _posOperation.MarkAsCompletedAndRememberDate();
                    break;
                case PosOperationStatus.PendingPayment:
                    MarkPosOperationAs(PosOperationStatus.Completed);
                    _posOperation.MarkAsPendingPayment();
                    break;
            }
        }

        public PosOperation Build()
        {
            return _posOperation;
        }
    }
}
