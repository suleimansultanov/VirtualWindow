using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Payment
{
    public class PaymentConfirmationDto
    {
        [Required]
        public int? TransactionId { get; set; }
        
        [Required]
        public string PaRes { get; set; }
    }
}