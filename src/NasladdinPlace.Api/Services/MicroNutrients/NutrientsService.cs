using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Api.Services.MicroNutrients.Contracts;
using NasladdinPlace.Api.Services.MicroNutrients.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MicroNutrients.Calculator;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.Utilities.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.MicroNutrients
{
    public class NutrientsService : INutrientsService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUserManager _userManager;
        private readonly INutrientsCalculator _calculator;

        public NutrientsService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _userManager = serviceProvider.GetRequiredService<IUserManager>();
            _calculator = serviceProvider.GetRequiredService<INutrientsCalculator>();
        }

        //TODO: отрефакторить метод
        public async Task<ValueResult<List<UserNutrientsHistory>>> GetNutrientsHistoryByUserAsync(int userId, DateTime workDate, byte countOfDays)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperations = await unitOfWork.PosOperations.GetNutrientsByUserAsync(userId, workDate, countOfDays);

                if (posOperations == null)
                {
                    var errorMessage = $"Pos Operations of user {userId} have not been found.";
                    LogError(errorMessage);
                    return ValueResult<List<UserNutrientsHistory>>.Failure(errorMessage);
                }

                var user = await _userManager.FindByIdAsync(userId);

                var calculatedNutrients = _calculator.GetNormalNutrientsByUser(user);
                var normalNutrients = calculatedNutrients.Succeeded ? calculatedNutrients.Value : new Nutrients();
                var userNutrientsHistories = new List<UserNutrientsHistory>();

                foreach (var (dateTime, operations) in posOperations)
                {
                    var nutrientsHistoryList = operations.Count > 0
                        ? operations.SelectMany(posOperation => posOperation.CheckItems)
                            .OrderBy(ci => ci.Id)
                            .GroupBy(checkItem => checkItem.PosOperation.DateCompleted.Value.Date)
                            .Select(g => new UserNutrientsHistory
                            {
                                Date = dateTime,
                                Nutrients = new Nutrients(g.Sum(ci => ci.GetGoodCalories()),
                                    g.Sum(ci => ci.GetGoodProteins()),
                                    g.Sum(ci => ci.GetGoodFats()),
                                    g.Sum(ci => ci.GetGoodCarbohydrates())),
                                Normal = normalNutrients,
                                Products = Mapper.Map<List<GoodWithPfccDto>>(g.Select(ci => ci.Good).ToList())
                            }).ToList()
                        : new List<UserNutrientsHistory>()
                        {
                            new UserNutrientsHistory
                            {
                                Date = dateTime,
                                Nutrients = new Nutrients(),
                                Normal = normalNutrients,
                                Products = new List<GoodWithPfccDto>()
                            }
                        };

                    userNutrientsHistories.AddRange(nutrientsHistoryList);
                }

                LogInformation($"Nutrients list builded for User {userId}.");

                return ValueResult<List<UserNutrientsHistory>>.Success(userNutrientsHistories);
            }
        }

        public async Task<ValueResult<Nutrients>> GetNutrientsByUserParametersAsync(UserParams userParams)
        {
            var user = await _userManager.FindByIdAsync(userParams.UserId);

            if (Enum.IsDefined(typeof(Gender), userParams.GenderEnum))
                user.Gender = userParams.GenderEnum;

            if (userParams.ActivityEnum.HasValue && Enum.IsDefined(typeof(ActivityType), userParams.ActivityEnum.Value))
                user.Activity = userParams.ActivityEnum.Value;

            if (userParams.GoalEnum.HasValue && Enum.IsDefined(typeof(GoalType), userParams.GoalEnum.Value))
                user.Goal = userParams.GoalEnum.Value;

            if (userParams.PregnancyEnum.HasValue && Enum.IsDefined(typeof(PregnancyType), userParams.PregnancyEnum.Value))
                user.Pregnancy = userParams.PregnancyEnum.Value;

            user.SetAge(userParams.Age);
            user.SetHeight(userParams.Height);
            user.SetWeight(userParams.Weight);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var error = $"Some error has been occured during update of user info a user {user.Id}: " +
                            $"{JsonConvert.SerializeObject(result.Errors)}";

                LogError(error);

                return ValueResult<Nutrients>.Failure(error);
            }

            var recommendedNutrients = _calculator.GetNormalNutrientsByUser(user);

            if (!recommendedNutrients.Succeeded)
                return ValueResult<Nutrients>.Success(new Nutrients());

            return ValueResult<Nutrients>.Success(recommendedNutrients.Value);
        }

        public async Task<ValueResult<UserNutrientsAndGoals>> GetUserGoalsAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var recommendedNutrients = _calculator.GetNormalNutrientsByUser(user);

            var userNutrientsAndGoal = new UserNutrientsAndGoals
            {
                Goals = new UserParams
                {
                    Activity = FirstCharacterToLower(user.Activity.ToString()),
                    Age = user.Age,
                    Gender = FirstCharacterToLower(user.Gender.ToString()),
                    Goal = FirstCharacterToLower(user.Goal.ToString()),
                    Height = user.Height,
                    Pregnancy = FirstCharacterToLower(user.Pregnancy.ToString()),
                    Weight = user.Weight
                },
                Nutrients = recommendedNutrients.Succeeded ? recommendedNutrients.Value : new Nutrients()
            };

            return ValueResult<UserNutrientsAndGoals>.Success(userNutrientsAndGoal);
        }

        private string FirstCharacterToLower(string enumName)
        {
            if (string.IsNullOrEmpty(enumName))
                return null;

            if (char.IsLower(enumName, 0))
                return enumName;

            return char.ToLowerInvariant(enumName[0]) + enumName.Substring(1);
        }

        private void LogInformation(string message)
        {
            _logger.Information(message);
        }

        private void LogError(string message)
        {
            _logger.Error(message);
        }
    }
}
