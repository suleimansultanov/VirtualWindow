using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.CommonModels;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheck
    {
        public static readonly SimpleCheck Empty = new SimpleCheck(
            default(int),
            Enumerable.Empty<SimpleCheckItem>(),
            DateTime.UtcNow,
            SimpleCheckOriginInfo.Unknown,
            SimpleCheckSummary.Default,
            CheckFiscalizationInfo.Empty,
            CheckCorrectnessStatus.Correct,
            CheckPaymentErrorInfo.Empty
        );
        
        private DateTime? _datePaid;
        
        public int Id { get; }
        public ICollection<SimpleCheckItem> Items { get; }
        public DateTime DateCreated { get; }
        public SimpleCheckSummary Summary { get; }
        public SimpleCheckOriginInfo OriginInfo { get; }
        public CheckFiscalizationInfo FiscalizationInfo { get; }
        public CheckCorrectnessStatus CorrectnessStatus { get; }
        public CheckPaymentErrorInfo PaymentErrorInfo { get; }

        public DateTime? DatePaid
        {
            get => _datePaid;
            set
            {
                if (value < DateCreated)
                    throw new ArgumentException(
                        "Date paid must be greater than date created. " +
                        $"But found: {value}."
                    );

                _datePaid = value;
            }
        }
        
        public SimpleCheck(
            int id,
            IEnumerable<SimpleCheckItem> items,
            DateTime dateCreated,
            SimpleCheckOriginInfo originInfo,
            SimpleCheckSummary summary,
            CheckFiscalizationInfo fiscalizationInfo,
            CheckCorrectnessStatus correctnessStatus,
            CheckPaymentErrorInfo paymentErrorInfo)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (dateCreated > DateTime.UtcNow)
                throw new ArgumentOutOfRangeException(
                    nameof(dateCreated),
                    dateCreated, 
                    $"Date created must be less than now. But found: {dateCreated}."
                );
            if (originInfo == null)
                throw new ArgumentNullException(nameof(originInfo));
            if (summary == null)
                throw new ArgumentNullException(nameof(summary));
            
            Id = id;
            Items = items.ToImmutableList();
            DateCreated = dateCreated;
            OriginInfo = originInfo;
            Summary = summary;
            FiscalizationInfo = fiscalizationInfo;
            CorrectnessStatus = correctnessStatus;
            PaymentErrorInfo = paymentErrorInfo;
        }

        public bool IsPaid => DatePaid != null;

        public bool IsFree => Summary.IsFreeCheck;

        public bool IsEmpty => Summary.IsEmptyCheck;

        public bool IsFreeOrEmpty => IsFree || IsEmpty;
    }
}
