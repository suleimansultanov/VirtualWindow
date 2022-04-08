using System.Runtime.Serialization;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Services.MicroNutrients.Models
{
    public class UserParams
    {
        public int UserId { get; private set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string Gender { get; set; }
        public string Goal { get; set; }
        public string Activity { get; set; }
        public string Pregnancy { get; set; }

        public void SetUserId(int userId)
        {
            UserId = userId;
        }

        [IgnoreDataMember]
        public Gender GenderEnum => string.IsNullOrEmpty(Gender)
            ? Core.Enums.Gender.Undefined
            : (Gender)System.Enum.Parse(typeof(Gender), Gender, ignoreCase: true);

        [IgnoreDataMember]
        public ActivityType? ActivityEnum => string.IsNullOrEmpty(Activity)
            ? (ActivityType?) null    
            : (ActivityType)System.Enum.Parse(typeof(ActivityType), Activity, ignoreCase: true);

        [IgnoreDataMember]
        public PregnancyType? PregnancyEnum => string.IsNullOrEmpty(Pregnancy)
            ? (PregnancyType?)null
            : (PregnancyType)System.Enum.Parse(typeof(PregnancyType), Pregnancy, ignoreCase: true);

        [IgnoreDataMember]
        public GoalType? GoalEnum => string.IsNullOrEmpty(Goal)
            ? (GoalType?)null
            : (GoalType)System.Enum.Parse(typeof(GoalType), Goal, ignoreCase: true);
    }
}
