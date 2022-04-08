using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.MicroNutrients.Calculator
{
    public interface INutrientsCalculator
    {
        ValueResult<Nutrients> GetNormalNutrientsByUser(ApplicationUser user);
    }
}
