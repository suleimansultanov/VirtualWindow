using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PaymentCard
{
    public class PaymentCardConfirmationDto
    {
        [Required]
        public string CardHolder { get; set; }
        
        [Required]
        public string CardCryptogramPacket { get; set; }

        public PaymentCardCryptogramSource? CryptogramSource { get; set; }
    }
}