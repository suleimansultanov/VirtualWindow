using System;
using NasladdinPlace.Api.Dtos.PaymentCard;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.User
{
    public class UserFullInfoDto : UserShortInfoDto
    {
        public string PhoneNumber { get; set; }
        
        public bool PhoneNumberConfirmed { get; set; }
        
        public PaymentCardDto ActivePaymentCard { get; set; }

        public decimal AvailableBonusPoints { get; set; }

        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public Gender Gender { get; set; }

        public GoalType? Goal { get; set; }

        public ActivityType? Activity { get; set; }

        public PregnancyType? Pregnancy { get; set; }

        public int? Age { get; set; }

        public int? Height { get; set; }

        public int? Weight { get; set; }
    }
}