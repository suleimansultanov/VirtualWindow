using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Dtos.BankTransaction
{
    public class BankTransactionInfoDto
    {
        public int Id { get; set; }
        
        [Required]
        public long? BankTransactionId { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        public BankTransactionInfoType Type { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Comment { get; set; }
        public long? PaybackId { get; set; }
    }
}