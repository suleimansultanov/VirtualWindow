using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.Payment.Card.Type;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Core.Models
{
    public class PaymentCard : Entity
    {
        public ApplicationUser User { get; private set; }
        
        public int UserId { get; private set; }
        public PaymentSystem PaymentSystem { get; private set; }
        public string Token { get; private set; }
        public PaymentCardCryptogramSource CryptogramSource { get; private set; }
        public string FirstSixDigits { get; private set; }
        public string LastFourDigits { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public PaymentCardStatus? Status { get; private set; }
        public DateTime? CreatedDate { get; private set; }

        protected PaymentCard()
        {
            // intentionally left empty
        }
        
        public PaymentCard(int userId, ExtendedPaymentCardInfo extendedPaymentCardInfo)
            : this(0, userId, extendedPaymentCardInfo)
        {
        }

        public PaymentCard(int id, int userId, ExtendedPaymentCardInfo extendedPaymentCardInfo)
            : this()
        {
            if (extendedPaymentCardInfo == null)
                throw new ArgumentNullException(nameof(extendedPaymentCardInfo));

            Id = id;
            UserId = userId;

            UpdateInfo(extendedPaymentCardInfo);
        }

        public void ResetCardToken()
        {
            Token = string.Empty;
        }

        public void UpdateInfo(ExtendedPaymentCardInfo extendedPaymentCardInfo)
        {
            if (extendedPaymentCardInfo == null)
                throw new ArgumentNullException(nameof(extendedPaymentCardInfo));

            PaymentSystem = extendedPaymentCardInfo.PaymentSystem;
            Token = extendedPaymentCardInfo.Token;
            CryptogramSource = extendedPaymentCardInfo.CryptogramSource;
            FirstSixDigits = extendedPaymentCardInfo.Number.FirstSixDigits;
            LastFourDigits = extendedPaymentCardInfo.Number.LastFourDigits;
            ExpirationDate = extendedPaymentCardInfo.ExpirationDate;
            CreatedDate = DateTime.UtcNow;
        }

        public void MarkAsAbleToMakePayment()
        {
            Status = PaymentCardStatus.AbleToMakePayment;
        }

        public void MarkAsNotAbleToMakePayment()
        {
            Status = PaymentCardStatus.NotAbleToMakePayment;
        }

        public void MarkAsDeleted()
        {
            Status = PaymentCardStatus.Deleted;
        }

        public void ResetNumberAndExpirationDate()
        {
            FirstSixDigits = null;
            LastFourDigits = null;
            ExpirationDate = null;
        }

        public bool HasNumber =>
            !string.IsNullOrWhiteSpace(FirstSixDigits) && !string.IsNullOrWhiteSpace(LastFourDigits);
        
        public PaymentCardNumber Number => HasNumber ? new PaymentCardNumber(FirstSixDigits, LastFourDigits) : null;

        public PaymentCardType Type => HasNumber
            ? PaymentCardTypeByNumberProvider.Provide(Number)
            : PaymentCardType.Unknown;
    }
}