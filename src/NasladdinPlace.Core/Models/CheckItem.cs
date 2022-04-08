using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Models.Interfaces;
using NasladdinPlace.Core.Services.Check.Discounts.Helpers;

namespace NasladdinPlace.Core.Models
{
    public class CheckItem : Entity, IReadonlyCheckItem
    {
        private decimal _price;
        private CheckItemStatus _checkItemStatus;
        private Currency _currency;
        private LabeledGood _labeledGood;

        public Good Good { get; private set; }
        public Pos Pos { get; private set; }
        public PosOperation PosOperation { get; private set; }
        public ICollection<CheckItemAuditRecord> AuditRecords { get; private set; }
        public ICollection<FiscalizationCheckItem> FiscalizationCheckItems { get; private set; }
        public ICollection<PosOperationTransactionCheckItem> PosOperationTransactionCheckItems { get; private set; }
        public int LabeledGoodId { get; private set; }
        public int CurrencyId { get; private set; }
        public int PosOperationId { get; private set; }
        public int PosId { get; private set; }
        public DateTime? DatePaid { get; private set; }
        public int GoodId { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal RoundedDiscountAmount => DiscountsHelper.Round(DiscountAmount);
        public decimal PriceWithDiscount => Price - RoundedDiscountAmount;
        public bool IsModifiedByAdmin { get; private set; }

        public Currency Currency
        {
            get => _currency;
            set => _currency = value ?? throw new ArgumentNullException($"{nameof(Currency)} value must not be null.");
        }

        public LabeledGood LabeledGood
        {
            get => _labeledGood;
            set => _labeledGood = value ?? throw new ArgumentNullException($"{nameof(LabeledGood)} value must not be null.");
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException($"Incorrect decimal {value} of the {nameof(Price)} property.");

                _price = value;
            }
        }

        public CheckItemStatus Status
        {
            get => _checkItemStatus;
            set
            {
                if (!Enum.IsDefined(typeof(CheckItemStatus), value))
                    throw new ArgumentException($"Incorrect value {value} of the {nameof(Status)} property.");

                _checkItemStatus = value;
            }
        }

        protected CheckItem()
        {
            AuditRecords = new Collection<CheckItemAuditRecord>();
            FiscalizationCheckItems = new Collection<FiscalizationCheckItem>();
            PosOperationTransactionCheckItems = new Collection<PosOperationTransactionCheckItem>();
        }

        public static CheckItemBuilder NewBuilder(int posId, int posOperationId, int goodId, int labeledGoodId, int currencyId)
        {
            return new CheckItemBuilder(posId, posOperationId, goodId, labeledGoodId, currencyId);
        }

        public CheckItem(int posId, int posOperationId, int goodId, int labeledGoodId, int currencyId) : this()
        {
            PosId = posId;
            PosOperationId = posOperationId;
            GoodId = goodId;
            LabeledGoodId = labeledGoodId;
            CurrencyId = currencyId;
        }

        public void MarkAsDeleted(int? userId = null)
        {
            if (Status != CheckItemStatus.Unverified && Status != CheckItemStatus.Unpaid)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as deleted. " +
                    $"Item must have {CheckItemStatus.Unverified.ToString()} or {CheckItemStatus.Unpaid.ToString()} status."
                );
            
            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.Deleted);
        }

        public void MarkAsRefunded(int? userId = null)
        {
            if (Status != CheckItemStatus.Paid && Status != CheckItemStatus.PaidUnverified)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as refunded. " +
                    $"Item must have {CheckItemStatus.Paid.ToString().ToLower()} or {CheckItemStatus.PaidUnverified.ToString()} status."
                );          

            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.Refunded);
        }

        public void MarkAsPaid(int? userId = null)
        {
            if (Status != CheckItemStatus.Unpaid && Status != CheckItemStatus.PaidUnverified)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid. " +
                    $"Item must have {CheckItemStatus.Unpaid.ToString()} or {CheckItemStatus.PaidUnverified.ToString()} status."
                );

            DatePaid = DateTime.UtcNow;
            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.Paid);
        }

        public void MarkAsUnpaid(int? userId = null)
        {
            if (Status != CheckItemStatus.Unverified)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as verified. " +
                    $"Item must have {CheckItemStatus.Unverified.ToString()} status."
                );

            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.Unpaid);
        }

        public void AddDiscount(decimal discountInPercentage)
        {
            DiscountAmount =  (Price * discountInPercentage) / 100;
        }

        public void MarkAsPaidUnverified(int? userId = null)
        {
            if (Status != CheckItemStatus.Paid)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid unverified. " +
                    $"Item must have {CheckItemStatus.Paid.ToString()} status."
                );

            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.PaidUnverified);
        }

        public void MarkAsUnverified(int? userId = null)
        {
            if (Status != CheckItemStatus.Deleted && Status != CheckItemStatus.Unpaid)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as unverified. " +
                    $"Item must have {CheckItemStatus.Deleted.ToString()} or {CheckItemStatus.Unpaid.ToString()} status."
                );

            UpdateCheckItemStatusAndAddAuditRecord(userId, CheckItemStatus.Unverified);
        }

        public void MarkAsModifiedByAdmin()
        {
            IsModifiedByAdmin = true;
        }

        public bool TryGetAuditHistoryRecord(out CheckItemAuditRecord auditRecord)
        {
            auditRecord = AuditRecords.FirstOrDefault(ah => ah.NewStatus == Status);

            return auditRecord != null;
        }

        public double GetGoodCalories()
        {
            return Good != null
                ? Good.GetCalories()
                : 0;
        }

        public double GetGoodProteins()
        {
            return Good != null
                ? Good.GetProteins()
                : 0;
        }

        public double GetGoodCarbohydrates()
        {
            return Good != null
                ? Good.GetCarbohydrates()
                : 0;
        }

        public double GetGoodFats()
        {
            return Good != null
                ? Good.GetFats()
                : 0;
        }

        private void UpdateCheckItemStatusAndAddAuditRecord(int? userId, CheckItemStatus newStatus)
        {
            var oldStatus = Status;

            var auditRecord = userId.HasValue
                ? CheckItemAuditRecord.ForUserOperation(Id, oldStatus, newStatus, userId.Value)
                : CheckItemAuditRecord.ForSystemOperation(Id, oldStatus, newStatus);

            AuditRecords.Add(auditRecord);

            Status = newStatus;
        }
    }
}
