using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Core.Services.Payment.Card
{
    public class ExtendedPaymentCardInfo : PaymentCardInfo
    {
        public PaymentCardCryptogramSource CryptogramSource { get; set; }
        public PaymentSystem PaymentSystem { get; set; }
        
        public ExtendedPaymentCardInfo(PaymentCardNumber number, DateTime expirationDate, string token) 
            : base(number, expirationDate, token)
        {
            CryptogramSource = PaymentCardCryptogramSource.Common;
            PaymentSystem = PaymentSystem.CloudPayments;
        }

        public ExtendedPaymentCardInfo(PaymentCardInfo paymentCardInfo)
            : this(paymentCardInfo.Number, paymentCardInfo.ExpirationDate, paymentCardInfo.Token)
        { 
        }
    }
}