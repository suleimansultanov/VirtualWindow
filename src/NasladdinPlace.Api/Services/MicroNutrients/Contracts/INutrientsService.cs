using NasladdinPlace.Api.Services.MicroNutrients.Models;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.MicroNutrients.Contracts
{
    public interface INutrientsService
    {
        Task<ValueResult<List<UserNutrientsHistory>>> GetNutrientsHistoryByUserAsync(int userId, DateTime workDate, byte countOfDays);
        Task<ValueResult<UserNutrientsAndGoals>> GetUserGoalsAsync(int userId);
        Task<ValueResult<Nutrients>> GetNutrientsByUserParametersAsync(UserParams userParams);
    }
}
