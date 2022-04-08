using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.BankTransaction
{
    public class BankTransactionInfoDto
    {
        public int Id { get; set; }
        
        [Required]
        public int? BankTransactionId { get; set; }

        [Required]
        public decimal? PaymentAmount { get; set; }

        public long? PaybackId { get; set; }
    }
}