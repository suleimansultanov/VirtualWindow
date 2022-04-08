using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Interfaces;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Models
{
    public class PosOperation : IReadonlyPosOperation
    {
        private ApplicationUser _user;
        private Pos _pos;
        private ICollection<CheckItem> _checkItems;
        private ICollection<PosOperationTransaction> _posOperationTransactions;
        private DateTime _dateStarted;
        private DateTime? _completionInitiationDate;

        public static PosOperationOfUserAndPosBuilder NewOfUserAndPosBuilder(int userId, int posId)
        {
            return new PosOperationOfUserAndPosBuilder(userId, posId);
        }

        public static PosOperation OfUserAndPos(int userId, int posId)
        {
            return new PosOperation
            {
                UserId = userId,
                PosId = posId
            };
        }

        public ICollection<BankTransactionInfo> BankTransactionInfos { get; private set; }
        public ICollection<PosOperationTransaction> PosOperationTransactions
        {
            get => _posOperationTransactions;
            set => _posOperationTransactions =
                value ?? throw new ArgumentNullException($"{nameof(PosOperationTransactions)} value must not be null.");
        }
        public ICollection<CheckItem> CheckItems
        {
            get => _checkItems;
            set => _checkItems =
                value ?? throw new ArgumentNullException($"{nameof(CheckItems)} value must not be null.");
        }

        public ICollection<LabeledGood> LabeledGoods { get; set; }
        public ICollection<PosDoorsState> PosDoorsStates { get; set; }
        public ICollection<FiscalizationInfo> FiscalizationInfos { get; set; }

        public ApplicationUser User
        {
            get => _user;
            set => _user = value ?? throw new ArgumentNullException($"{nameof(User)} value must not be null.");
        }

        public Pos Pos
        {
            get => _pos;
            set => _pos = value ?? throw new ArgumentNullException($"{nameof(Pos)} value must not be null.");
        }

        public int Id { get; set; }

        public int UserId { get; private set; }
        public int PosId { get; private set; }

        public DateTime DateStarted
        {
            get => _dateStarted;
            set
            {
                if (value > DateTime.UtcNow)
                        throw new ArgumentException($"Incorrect datetime {value} of the {nameof(DateStarted)} property.");
                _dateStarted = value;
            }
        }

        public DateTime? DatePaid { get; private set; }
        public DateTime? DateCompleted { get; private set; }
        public DateTime? CompletionInitiationDate
        {
            get => _completionInitiationDate;
            set
            {
                if (value > DateTime.UtcNow)
                    throw new ArgumentException($"Incorrect datetime {value} of the {nameof(CompletionInitiationDate)} property.");
                _completionInitiationDate = value;
            }
        }
        public PosOperationStatus Status { get; private set; }
        public decimal BonusAmount { get; private set; }
        public Brand Brand { get; set; }
        public PosMode Mode { get; set; }
        public DateTime? AuditRequestDateTime { get; private set; }
        public DateTime? AuditCompletionDateTime { get; private set; }
        public CheckCorrectnessStatus CorrectnessStatus { get; private set; }
        public bool IsPaid => Mode == PosMode.Purchase && Status == PosOperationStatus.Paid;
        public ICollection<DocumentGoodsMoving> DocumentsGoodsMoving { get; set; }

        protected PosOperation()
        {
            CheckItems = new Collection<CheckItem>();
            LabeledGoods = new Collection<LabeledGood>();
            PosDoorsStates = new Collection<PosDoorsState>();
            BankTransactionInfos = new Collection<BankTransactionInfo>();
            FiscalizationInfos = new Collection<FiscalizationInfo>();
            PosOperationTransactions = new Collection<PosOperationTransaction>();
            DocumentsGoodsMoving = new Collection<DocumentGoodsMoving>();
            DateStarted = DateTime.UtcNow;
            Brand = Brand.Invalid;
            CorrectnessStatus = CheckCorrectnessStatus.NotChecked;
        }

        public PosOperation(
            int id,
            int userId,
            int posId,
            DateTime dateStarted,
            DateTime? datePaid,
            DateTime? completionInitiationDate,
            DateTime? dateCompleted)
            : this()
        {
            Id = id;
            UserId = userId;
            PosId = posId;
            DateStarted = dateStarted;
            DatePaid = datePaid;
            DateCompleted = dateCompleted;
            CompletionInitiationDate = completionInitiationDate;
        }

        public void MarkAsPendingCompletion()
        {
            if (Status != PosOperationStatus.Opened)
                throw new ArgumentException(
                    $"Cannot mark {Status.ToString()} as pending completion. " +
                    $"Item must have status {PosOperationStatus.Opened.ToString().ToLower()}."
                );
            
            Status = PosOperationStatus.PendingCompletion;
            CompletionInitiationDate = DateTime.UtcNow;
        }

        public bool IsCheckCreationForbidden =>
            Status != PosOperationStatus.Opened && Status != PosOperationStatus.PendingCompletion;

        public void SubtractBonus(decimal value)
        {
            BonusAmount = BonusAmount - value > 0 ? BonusAmount - value : 0;
        }

        public void AddBonusPoints(decimal value)
        {
            BonusAmount += value;
        }

        public void MarkAsCompletedAndRememberDate()
        {
            DateCompleted = DateTime.UtcNow;
            MarkAsCompleted();
        }

        public void MarkAsCompleted()
        {
            if (Status != PosOperationStatus.PendingCheckCreation && Status != PosOperationStatus.PendingPayment)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as completed. " +
                    $"Item must have {PosOperationStatus.PendingCheckCreation.ToString().ToLower()} or {PosOperationStatus.PendingPayment.ToString().ToLower()} status."
                );

            Status = PosOperationStatus.Completed;
        }

        public void MarkAsPendingCheckCreation()
        {
            if (IsCheckCreationForbidden)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as pending check creation or opened. " +
                    $"Item must have {PosOperationStatus.PendingCompletion.ToString().ToLower()} or " +
                    $"{PosOperationStatus.Opened.ToString().ToLower()} status."
                );

            Status = PosOperationStatus.PendingCheckCreation;
        }

        public void MarkAsPendingPayment()
        {
            if (Status != PosOperationStatus.Completed)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as pending payment. " +
                    $"Item must have {PosOperationStatus.Completed.ToString().ToLower()} status."
                );

            Status = PosOperationStatus.PendingPayment;
        }

        public void MarkAuditRequested()
        {
            AuditRequestDateTime = DateTime.UtcNow;            
        }

        public void MarkAuditCompleted()
        {
            AuditCompletionDateTime = DateTime.UtcNow;
        }
        
        public void MarkAsPaid(OperationPaymentInfo operationPaymentInfo)
        {
            if (operationPaymentInfo == null)
                throw new ArgumentNullException(nameof(operationPaymentInfo));

            if (Status != PosOperationStatus.PendingPayment)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid. " +
                    $"Item must have {PosOperationStatus.PendingPayment.ToString().ToLower()} status."
                );

            if (operationPaymentInfo.HasBankTransactionSummary)
            {
                var bankTransactionSummary = operationPaymentInfo.BankTransactionSummary;
                var paymentTransactionInfo = BankTransactionInfo.CreateFromSummary(bankTransactionSummary);
                BankTransactionInfos.Add(paymentTransactionInfo);
            }

            DatePaid = DateTime.UtcNow;
            Status = PosOperationStatus.Paid;

            var notPaidCheckItems = FindCheckItemsWithStatuses(CheckItemStatus.Unpaid);
            foreach (var notPaidCheckItem in notPaidCheckItems)
            {
                notPaidCheckItem.MarkAsPaid();
            }
        }

        public void MarkAsPaid()
        {
            if (Status != PosOperationStatus.PendingPayment)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid. " +
                    $"Item must have {PosOperationStatus.PendingPayment.ToString().ToLower()} status."
                );

            DatePaid = DateTime.UtcNow;
            Status = PosOperationStatus.Paid;
        }

        public void SetCorrectnessStatus(CheckCorrectnessStatus correctnessStatus)
        {
            CorrectnessStatus = correctnessStatus;
        }

        public void AddCheckItems()
        {
            foreach (var labeledGood in LabeledGoods)
            {
                if (!labeledGood.Price.HasValue || !labeledGood.GoodId.HasValue || !labeledGood.CurrencyId.HasValue)
                {
                    continue;
                }

                var status = labeledGood.IsDisabled ? CheckItemStatus.Unverified : CheckItemStatus.Unpaid;

                var checkItem = CheckItem.NewBuilder(
                        PosId, 
                        Id, 
                        labeledGood.GoodId.Value,
                        labeledGood.Id,
                        labeledGood.CurrencyId.Value)
                    .SetPrice(labeledGood.Price.Value)
                    .SetStatus(status)
                    .Build();

                CheckItems.Add(checkItem);
            }
        }

        public void AddCheckItem(CheckItem chekcItem)
        {
            if (chekcItem == null)
                throw new ArgumentNullException(nameof(chekcItem));

            CheckItems.Add(chekcItem);
        }

        public IEnumerable<PaymentCardCryptogramSource> CryptogramSources
            => BankTransactionInfos
                .Where(bti => bti.PaymentCard != null)
                .Select(bti => bti.PaymentCard.CryptogramSource)
                .ToImmutableSortedSet();

        public IEnumerable<CheckItem> FindCheckItemsWithStatuses(params CheckItemStatus[] checkItemStatuses)
        {
            return CheckItems.Where(ci => checkItemStatuses.Contains(ci.Status)).ToImmutableList();
        }

        public IEnumerable<CheckItem> FindCheckItemsWithStatusesByIds(
            IEnumerable<int> ids, params CheckItemStatus[] checkItemStatuses)
        {
            return FindCheckItemsWithStatuses(checkItemStatuses).Where(ci => ids.Contains(ci.Id)).ToImmutableList();
        }
        
        public bool CanBeContinuedByUser(int userId, PosMode mode)
        {
            return DoesBelongToUserInMode(userId, mode) && IsActive;
        }

        public bool DoesBelongToUserInMode(int userId, PosMode mode)
        {
            return UserId == userId && Mode == mode;
        }
        
        public bool IsActive => Status == PosOperationStatus.Opened;
        
        public bool TryGetCheckFiscalizationInfo(
            string fiscalizationQrCodeLinkFormat, 
            string fiscalCheckLinkFormat,
            out CheckFiscalizationInfo checkFiscalizationInfo)
        {
            if (fiscalizationQrCodeLinkFormat == null)
                throw new ArgumentNullException(nameof(fiscalizationQrCodeLinkFormat));
            
            checkFiscalizationInfo = FiscalizationInfos
                .Where(fi => fi.IsSuccessfulAndFullOfTypeIncome)
                .OrderBy(fi => fi.DateTimeRequest)
                .FirstOrDefault()?
                .ToCheckFiscalizationInfo(fiscalizationQrCodeLinkFormat, fiscalCheckLinkFormat);

            return checkFiscalizationInfo != null;
        }

        public bool TryGetFiscalizationInfoByCheckItem(
            string fiscalizationQrCodeLinkFormat, 
            string fiscalCheckLinkFormat,
            int checkItemId, 
            out CheckFiscalizationInfo checkFiscalizationInfo)
        {
            checkFiscalizationInfo = FiscalizationInfos
                .FirstOrDefault(fi => fi.CheckWhetherSuccessfulAndFullOfTypeIncomeRefundRelatedToCheckItem(checkItemId))?
                .ToCheckFiscalizationInfo(fiscalizationQrCodeLinkFormat, fiscalCheckLinkFormat);

            return checkFiscalizationInfo != null;
        }

        public void AddTransaction(PosOperationTransactionType transactionType)
        {
            PosOperationTransaction posOperationTransaction;

            switch (transactionType)
            {
                case PosOperationTransactionType.RegularPurchase:
                    posOperationTransaction = PosOperationTransaction
                        .NewPaymentOfPosOperationBuilder(this).Build();
                    break;
                case PosOperationTransactionType.Addition:
                    posOperationTransaction = PosOperationTransaction
                        .NewAdditionOfPosOperationBuilder(this).Build();
                    break;
                case PosOperationTransactionType.Refund:
                    posOperationTransaction = PosOperationTransaction
                        .NewRefundOfPosOperationBuilder(this).Build();
                    break;
                case PosOperationTransactionType.Verification:
                default:
                    throw new ArgumentException(nameof(PosOperationTransactionType));
            }

            PosOperationTransactions.Add(posOperationTransaction);
        }

        public void AddTransaction(PosOperationTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            PosOperationTransactions.Add(transaction);
        }

        public void AddBankTransaction(BankTransactionInfo bankTransaction)
        {
            if (bankTransaction == null)
                throw new ArgumentNullException(nameof(bankTransaction));

            BankTransactionInfos.Add(bankTransaction);
        }

        public ValueResult<PosOperationTransaction> GetTransaction(PosOperationTransactionType transactionType)
        {
            var operationTransaction = PosOperationTransactions
                .OrderByDescending(pot => pot.CreatedDate)
                .FirstOrDefault(pot => pot.Type == transactionType);

            if (operationTransaction != null)
                return ValueResult<PosOperationTransaction>.Success(operationTransaction);

            return ValueResult<PosOperationTransaction>.Failure($"Can not find PosOperationTransaction for PosOperationId = {Id}");
        }

        public string GetAbbreviatedPosName()
        {
            return Pos.AbbreviatedName;
        }

        public bool GetNewPaymentSystemFlag()
        {
            return Pos.UseNewPaymentSystem;
        }

        public IEnumerable<PosOperationTransaction> GetUnpaidTransactions()
        {
            return PosOperationTransactions
                .Where(pot => pot.Status == PosOperationTransactionStatus.Unpaid)
                .OrderBy(pot => pot.CreatedDate)
                .ToImmutableList();
        }

        private sealed class IdEqualityComparer : IEqualityComparer<PosOperation>
        {
            public bool Equals(PosOperation x, PosOperation y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(PosOperation obj)
            {
                return obj.Id;
            }
        }

        public static IEqualityComparer<PosOperation> IdComparer { get; } = new IdEqualityComparer();

        public void WriteOffBonusPoints()
        {
            if (User == null)
                throw new NotSupportedException($"The property {nameof(User)} can not be null");

            var unpaidCheckItems = FindCheckItemsWithStatuses(CheckItemStatus.Unpaid).ToImmutableList();
            var unpaidCheckItemsSumWithDiscount = unpaidCheckItems.Sum(cki => cki.PriceWithDiscount);

            var paymentInfo = new CheckPaymentInfo(unpaidCheckItemsSumWithDiscount, User.TotalBonusPoints);
            BonusAmount = paymentInfo.CheckCostInBonuses;

            User.SubtractBonusPoints(BonusAmount, BonusType.Payment);
        }

        public BankTransactionInfo GetLastBankTransactionInfo()
        {
	        return BankTransactionInfos.OrderByDescending( t => t.DateCreated ).FirstOrDefault();
        }
    }
}
