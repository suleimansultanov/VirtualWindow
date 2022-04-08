using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Interfaces;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Fiscalization.Models;

namespace NasladdinPlace.Core.Models
{
    public class PosOperationTransaction : Entity
    {
        public PosOperation PosOperation { get; private set; }
        public int PosOperationId { get; private set; }
        public Pos Pos { get; private set; }
        public int PosId { get; private set; }
        public BankTransactionInfoVersionTwo LastBankTransactionInfo { get; private set; }
        public int? LastBankTransactionInfoId { get; private set; }
        public ICollection<BankTransactionInfoVersionTwo> BankTransactionInfos { get; private set; }
        //TODO: make private set
        public FiscalizationInfoVersionTwo LastFiscalizationInfo { get; set; }
        public int? LastFiscalizationInfoId { get; private set; }
        public ICollection<FiscalizationInfoVersionTwo> FiscalizationInfos { get; private set; }
        public decimal BonusAmount { get; private set; }
        public decimal MoneyAmount { get; private set; }
        //TODO: make private set
        public decimal FiscalizationAmount { get; set; }
        public DateTime? BankTransactionPaidDate { get; private set; }
        //TODO: make private set
        public DateTime? FiscalizationDate { get; set; }
        public PosOperationTransactionType Type { get; private set; }
        public PosOperationTransactionStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public ICollection<UserBonusPoint> BonusPoints { get; private set; }
        public ICollection<PosOperationTransactionCheckItem> PosOperationTransactionCheckItems { get; private set; }
        public decimal TotalCost { get; private set; }
        public decimal TotalDiscountAmount { get; private set; }

        protected PosOperationTransaction()
        {
            BonusPoints = new Collection<UserBonusPoint>();
            PosOperationTransactionCheckItems = new Collection<PosOperationTransactionCheckItem>();
            BankTransactionInfos = new Collection<BankTransactionInfoVersionTwo>();
            FiscalizationInfos = new Collection<FiscalizationInfoVersionTwo>();
        }

        public static PosOperationTransaction ForPayment(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransaction(posOperation, PosOperationTransactionType.RegularPurchase);
        }

        public static PosOperationTransaction ForRefund(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransaction(posOperation, PosOperationTransactionType.Refund);
        }

        public static PosOperationTransaction ForAddition(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransaction(posOperation, PosOperationTransactionType.Addition);
        }

        public static PosOperationTransaction ForVerification(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransaction(posOperation, PosOperationTransactionType.Verification);
        }

        private PosOperationTransaction(
            IReadonlyPosOperation posOperation,
            PosOperationTransactionType transactionType) : this()
        {
            if (!Enum.IsDefined(typeof(PosOperationTransactionType), transactionType))
                throw new ArgumentException(nameof(transactionType));

            PosOperationId = posOperation.Id;
            PosId = posOperation.PosId;
            Type = transactionType;
            Status = PosOperationTransactionStatus.Unpaid;
            CreatedDate = DateTime.UtcNow;
        }

        public static PosOperationTransactionBuilder NewPaymentOfPosOperationBuilder(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransactionBuilder(posOperation, PosOperationTransactionType.RegularPurchase);
        }

        public static PosOperationTransactionBuilder NewRefundOfPosOperationBuilder(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransactionBuilder(posOperation, PosOperationTransactionType.Refund);
        }

        public static PosOperationTransactionBuilder NewAdditionOfPosOperationBuilder(IReadonlyPosOperation posOperation)
        {
            return new PosOperationTransactionBuilder(posOperation, PosOperationTransactionType.Addition);
        }

        public void SubtractBonus(decimal value)
        {
            BonusAmount = BonusAmount - value > 0 ? BonusAmount - value : 0;
        }

        public void AddBonus(decimal value)
        {
            BonusAmount += value;
        }

        public void MarkAsInProcess()
        {
            if (Status != PosOperationTransactionStatus.Unpaid && Status != PosOperationTransactionStatus.PaidUnfiscalized)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as in process. " +
                    $"Item must have {PosOperationTransactionStatus.Unpaid.ToString().ToLower()} or {PosOperationTransactionStatus.PaidUnfiscalized.ToString().ToLower()} status."
                );

            Status = PosOperationTransactionStatus.InProcess;
        }

        public void MarkAsPaid(OperationPaymentInfo operationPaymentInfo)
        {
            if (operationPaymentInfo == null)
                throw new ArgumentNullException(nameof(operationPaymentInfo));

            if (Status != PosOperationTransactionStatus.Unpaid && Status != PosOperationTransactionStatus.InProcess)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid unfiscalized. " +
                    $"Item must have {PosOperationTransactionStatus.Unpaid.ToString().ToLower()} or {PosOperationTransactionStatus.InProcess.ToString().ToLower()} status."
                );

            if (operationPaymentInfo.HasBankTransactionSummary)
            {
                var bankTransactionSummary = operationPaymentInfo.BankTransactionSummary;
                var bankTransactionInfoVersionTwo = BankTransactionInfoVersionTwo.CreateFromSummary(bankTransactionSummary);
                BankTransactionInfos.Add(bankTransactionInfoVersionTwo);
                LastBankTransactionInfo = bankTransactionInfoVersionTwo;
                BankTransactionPaidDate = bankTransactionInfoVersionTwo.DateCreated;
                Status = PosOperationTransactionStatus.PaidUnfiscalized;
            }
            else
            {
                MarkAsPaidByBonusPoints();
            }
        }

        public void MarkAsUnpaid()
        {
            if (Status != PosOperationTransactionStatus.InProcess)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as unpaid. " +
                    $"Item must have {PosOperationTransactionStatus.InProcess.ToString().ToLower()} status."
                );

            Status = PosOperationTransactionStatus.Unpaid;
        }

        public void MarkAsPaidUnfiscalized()
        {
            if (Status != PosOperationTransactionStatus.InProcess)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid unfiscalized. " +
                    $"Item must have {PosOperationTransactionStatus.InProcess.ToString().ToLower()} status."
                );

            Status = PosOperationTransactionStatus.PaidUnfiscalized;
        }

        public void MarkAsPaidFiscalized()
        {
            if (Status != PosOperationTransactionStatus.PaidUnfiscalized)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid fiscalized. " +
                    $"Item must have {PosOperationTransactionStatus.PaidUnfiscalized.ToString().ToLower()} status."
                );

            Status = PosOperationTransactionStatus.PaidFiscalized;
        }

        public void MarkAsPaidByBonusPoints()
        {
            if (Status != PosOperationTransactionStatus.InProcess)
                throw new InvalidOperationException(
                    $"Cannot mark {Status.ToString()} item as paid by bonuses. " +
                    $"Item must have {PosOperationTransactionStatus.InProcess.ToString().ToLower()} status."
                );

            Status = PosOperationTransactionStatus.PaidByBonusPoints;
        }

        public void SetBankTransactionRequisites()
        {
            var lastBankTransactionInfo = PosOperation.BankTransactionInfos.LastOrDefault();

            if (lastBankTransactionInfo == null) return;

            BankTransactionPaidDate = lastBankTransactionInfo.DateCreated;
            LastBankTransactionInfoId = lastBankTransactionInfo.Id;
        }

        public void SetFiscalizationDate(DateTime? fiscalizationDate)
        {
            FiscalizationDate = fiscalizationDate;
        }

        public void AddTransactionCheckItems(
            IPosOperationTransactionCheckItemsMaker transactionCheckItemsMaker,
            IReadOnlyList<IReadonlyCheckItem> checkItems,
            decimal bonusPoints)
        {
            var posOperationTransactionCheckItems = transactionCheckItemsMaker.MakeCheckItems(
                bonusPoints, checkItems);
            posOperationTransactionCheckItems.ForEach(pcki => PosOperationTransactionCheckItems.Add(pcki));
        }

        public void AddBankTransaction(BankTransactionInfoVersionTwo bankTransaction)
        {
            if (bankTransaction == null)
                throw new ArgumentNullException(nameof(bankTransaction));

            BankTransactionPaidDate = bankTransaction.DateCreated;
            LastBankTransactionInfo = bankTransaction;
            BankTransactionInfos.Add(bankTransaction);
        }

        public void MarkPosOperationAsPendingPayment()
        {
            PosOperation.MarkAsPendingPayment();
        }

        public void SetLastFiscalizationInfo(FiscalizationInfoVersionTwo fiscalizationInfo)
        {
            LastFiscalizationInfo = fiscalizationInfo;
            FiscalizationInfos.Add(fiscalizationInfo);
        }

        public void SetAdditionalFiscalizationRequisites(decimal? amount, DateTime? documentDateTime)
        {
            FiscalizationDate = documentDateTime;
        }

        public void CalculateAndSetAmounts(IReadOnlyCollection<IReadonlyCheckItem> checkItems, decimal bonusPoints)
        {
            var transactionAmountInfo = new PosOperationTransactionAmountInfo(checkItems, bonusPoints);

            TotalDiscountAmount = transactionAmountInfo.TotalDiscount;
            TotalCost = transactionAmountInfo.CheckCostWithoutDiscountAndBonusPoints;
            BonusAmount = transactionAmountInfo.CheckCostInBonuses;
            MoneyAmount = FiscalizationAmount = transactionAmountInfo.CheckCostInMoney;
        }

        public decimal GetTotalCostWithDiscount()
        {
            var totalCostWithDiscount = TotalCost - TotalDiscountAmount;

            if (totalCostWithDiscount < 0)
                throw new ArgumentException("Total cost with discount must be greater or equal 0");

            return totalCostWithDiscount;
        }

        public List<IOnlineCashierProduct> GetCheckOnlineRequestProducts()
        {
            var transactionCheckItems = PosOperationTransactionCheckItems.ToImmutableList();

            return transactionCheckItems.Select(cki => (IOnlineCashierProduct)new CheckOnlineRequestProduct
            {
                Name = cki.GetGoodName(),
                Amount = cki.Amount,
                Count = 1
            }).ToList();
        }

        public List<FiscalItem> GetCloudKassirRequestFiscalItems()
        {
            var transactionCheckItems = PosOperationTransactionCheckItems.ToImmutableList();

            return transactionCheckItems
                .Select(cki => new FiscalItem(
                    label: cki.GetGoodName(), 
                    price: cki.Amount, 
                    quantity: 1, 
                    vat: VatValues.P0))
                .ToList();
        }

        public void RemoveTransactionCheckItems()
        {
            PosOperationTransactionCheckItems.Clear();
        }
    }
}
