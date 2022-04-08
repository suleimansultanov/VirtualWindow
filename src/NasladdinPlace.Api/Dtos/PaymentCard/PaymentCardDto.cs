using System;
using NasladdinPlace.Core.Services.Payment.Card.Type;

namespace NasladdinPlace.Api.Dtos.PaymentCard
{
    public class PaymentCardDto
    {
        public int Id { get; set; }

        public PaymentCardNumberDto Number { get; set; }

        public DateTime? ExpirationDate { get; set; }
        
        public PaymentCardType Type { get; set; }
        public bool IsActive { get; set; }
    }
}