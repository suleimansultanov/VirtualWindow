using NasladdinPlace.Core.Enums;
using NasladdinPlace.UI.Dtos.PosOperation;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.Dtos.User
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public DateTime RegistrationInitiationDate { get; set; }
        public DateTime? PaymentCardVerificationInitiationDate { get; set; }
        public DateTime? PaymentCardVerificationCompletionDate { get; set; }
        public DateTime? RegistrationCompletionDate { get; set; }
        public Gender Gender { get; set; }
        public decimal? TotalBonus { get; set; }
        public ShortenPosOperationDto LastPosOperation { get; set; }
    }
}