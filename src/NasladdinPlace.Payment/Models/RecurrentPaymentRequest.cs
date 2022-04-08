using System;

namespace NasladdinPlace.Payment.Models
{
    public class RecurrentPaymentRequest
    {
        private string _description;

        public decimal Amount { get; }
        public Currency Currency { get; }
        public string CardToken { get; }
        public string UserIdentifier { get; }

        public string Description
        {
            get => _description;
            set => _description =
                value ?? throw new ArgumentNullException($"{nameof(Description)} value must not be null.");
        }

        public RecurrentPaymentRequest(
            decimal amount,
            Currency currency,
            string cardToken, 
            string userIdentifier)
        {
            Amount = amount;
            Currency = currency;
            CardToken = cardToken;
            UserIdentifier = userIdentifier;
        }
    }
}