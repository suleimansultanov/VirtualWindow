using NasladdinPlace.Core.Services.MicroNutrients.Models;

namespace NasladdinPlace.Api.Services.MicroNutrients.Models
{
    public class UserNutrientsAndGoals
    {
        public UserParams Goals { get; set; }
        public Nutrients Nutrients { get; set; }
    }
}
