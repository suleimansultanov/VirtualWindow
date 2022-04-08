using System;

namespace NasladdinPlace.Payment.Models
{
    public class PaymentRequest
    {
        private string _description;

        public decimal Amount { get; }
        public Currency Currency { get; }
        public string CardCryptogramPacket { get; }
        public string CardHolderName { get; }
        public string CardHolderIpAddress { get; }
        public string UserIdentifier { get; }

        public string Description
        {
            get => _description;
            set => _description =
                value ?? throw new ArgumentNullException($"{nameof(Description)} value must not be null.");
        }

        public PaymentRequest(
            decimal amount,
            Currency currency,
            string cardCryptogramPacket,
            string cardHolderName,
            string cardHolderIpAddress,
            string userIdentifier)
        {
            Amount = amount;
            Currency = currency;
            CardCryptogramPacket = cardCryptogramPacket;
            CardHolderName = cardHolderName;
            CardHolderIpAddress = cardHolderIpAddress;
            UserIdentifier = userIdentifier;
        }
    }
}