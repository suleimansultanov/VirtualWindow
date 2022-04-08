using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheck
    {
        public ICollection<DetailedCheckGood> CheckGoods { get; }
        public DetailedCheckSummary Summary { get; }
        public DetailedCheckUserInfo UserInfo { get; }
        public DetailedCheckPosInfo PosInfo { get; }
        public PosOperationStatus Status { get; }
        public DateTime DateStatusUpdated { get; }
        public DetailedCheckGoodsStatistics Statistics { get; }
        public CheckFiscalizationInfo FiscalizationInfo { get; }
        public CheckCorrectnessStatus CorrectnessStatus { get; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DatePaid { get; set; }
        public CheckPaymentErrorInfo PaymentErrorInfo { get; }

        public IEnumerable<PaymentCardCryptogramSource> CryptogramSources
        {
            get => _cryptogramSources;
            set
            {
                if (value == null) return;

                _cryptogramSources = value.ToImmutableList();
            }
        }

        private ICollection<PaymentCardCryptogramSource> _cryptogramSources;

        public int Id { get; }

        public DetailedCheck(
            PosOperation operation,
            ICollection<DetailedCheckGood> checkGoods,
            DetailedCheckUserInfo userInfo,
            DetailedCheckPosInfo posInfo,
            DetailedCheckGoodsStatistics statistics,
            CheckFiscalizationInfo fiscalizationInfo,
            CheckPaymentErrorInfo paymentErrorInfo)
        {
            Id = operation.Id;
            CheckGoods = checkGoods.ToImmutableList();
            Summary = DetailedCheckSummary.FromGoods(checkGoods, operation.BonusAmount);
            UserInfo = userInfo;
            Statistics = statistics;
            PosInfo = posInfo;
            Status = operation.Status;
            DateStatusUpdated = operation.DatePaid ?? operation.DateCompleted ?? operation.DateStarted;
            CryptogramSources = new Collection<PaymentCardCryptogramSource>();
            FiscalizationInfo = fiscalizationInfo;
            CorrectnessStatus = operation.CorrectnessStatus;
            PaymentErrorInfo = paymentErrorInfo;
        }
    }
}