using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Services.MicroNutrients.Contracts;
using NasladdinPlace.Api.Services.MicroNutrients.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.MicroNutrients.DataGenerators;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Scenarios.MicroNutrients
{
    public class NutrientsTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;

        private INutrientsService _nutrientsService;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            _nutrientsService = serviceProvider.GetRequiredService<INutrientsService>();
        }

        [TestCaseSource(typeof(NutrientsHistoryDataGenerator))]
        public void GetNutrientsHistory_ContentWithPosOperation_ShoudReturnHistoryByDate(
            Collection<CheckItem> checkItems,
            byte countOfDays,
            UserParams userParams)
        {
            SeedData(checkItems);

            var startDate = DateTime.UtcNow;

            userParams.SetUserId(DefaultUserId);

            var userGoalsSettingResult = _nutrientsService.GetNutrientsByUserParametersAsync(userParams).GetAwaiter().GetResult();

            userGoalsSettingResult.Succeeded.Should().BeTrue();

            EnsureProteinsFatsCarbohydratesCaloriesServiceHasReturnedExpectedResult(
                DefaultUserId,
                startDate.Date,
                countOfDays);
        }

        [TestCaseSource(typeof(UserGoalsWithStringParamsDataGenerator))]
        public void GetNutrientsByUserParameters_UserGoalsAreGiven_ShoudReturnUserParamtersWithStringNaming(
            UserParams userParams)
        {
            userParams.SetUserId(DefaultUserId);

            var userGoalsSettingResult = _nutrientsService.GetNutrientsByUserParametersAsync(userParams).GetAwaiter().GetResult();

            userGoalsSettingResult.Succeeded.Should().BeTrue();
            EnsurePatchedUserHasExpectedResult(
                DefaultUserId,
                userParams);
        }

        [TestCaseSource(typeof(NutrientsDataGenerator))]
        public void GetNutrients_UserParametersGiven_ShoudReturnRecommendedNutrients(
            UserParams userParams, Nutrients expectedNutrients)
        {
            userParams.SetUserId(DefaultUserId);

            var gettingUserNutrientsResult = _nutrientsService.GetNutrientsByUserParametersAsync(userParams).GetAwaiter().GetResult();

            EnsurePatchedUserHasExpectedResult(DefaultUserId, userParams);
            EnsureNutrientsHasExpectedNutrientsResult(gettingUserNutrientsResult.Value, expectedNutrients);
        }

        [TestCaseSource(typeof(NutrientsDataGenerator))]
        public void GetRecomendedNutrients_ByUserGoals_ShoudReturnRecomendedNutrientsForUser(
            UserParams userParams,
            Nutrients expectedNutrients)
        {
            userParams.SetUserId(DefaultUserId);

            var usersRecomendedNutrientsByGoals =
                _nutrientsService
                    .GetNutrientsByUserParametersAsync(userParams)
                    .GetAwaiter().GetResult();

            EnsureNutrientsHasExpectedNutrientsResult(
                usersRecomendedNutrientsByGoals.Value,
                expectedNutrients);
        }

        private void EnsureProteinsFatsCarbohydratesCaloriesServiceHasReturnedExpectedResult(
            int userId,
            DateTime startDate,
            byte countOfDays)
        {
            var nutrientsHistoryResult =
                _nutrientsService.GetNutrientsHistoryByUserAsync(userId, startDate, countOfDays)
                    .GetAwaiter().GetResult();

            EnsureNutrientsServiceHasReturnedExpectedHistoryResult(
                nutrientsHistoryResult.Value,
                countOfDays);
        }

        private void EnsureNutrientsServiceHasReturnedExpectedHistoryResult(
            List<UserNutrientsHistory> nutrientsHistoryResult,
            byte countOfDays)
        {
            nutrientsHistoryResult.Count.Should().Be(countOfDays);

            foreach (var userNutrientsHistory in nutrientsHistoryResult)
            {
                userNutrientsHistory.Normal.Should().NotBeNull();
                userNutrientsHistory.Nutrients.Should().NotBeNull();
                userNutrientsHistory.Products.Should().NotBeNull();
                userNutrientsHistory.Date.Should().BeIn(DateTimeKind.Utc);
                userNutrientsHistory.Products.Count.Should().BeGreaterThan(0);
            }
        }

        private List<PosOperation> CreatePaidPosOperationAndSetCompletedDate(IEnumerable<CheckItem> checkItems)
        {
            var posOperations = new List<PosOperation>();
            for (var i = 0; i < checkItems.Count(); i++)
            {
                var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .MarkAsPendingPayment()
                    .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                    .Build();
                posOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-i));

                posOperations.Add(posOperation);
            }

            return posOperations;
        }

        private void SeedData(IEnumerable<CheckItem> checkItems)
        {
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var posOperations = CreatePaidPosOperationAndSetCompletedDate(checkItems);

            Seeder.Seed(posOperations);

            Seeder.Seed(checkItems);
        }

        public void EnsureNutrientsHasExpectedNutrientsResult(Nutrients calculatedNutrients, Nutrients expectedNutrients)
        {
            calculatedNutrients.Calories.Should().Be(expectedNutrients.Calories);
            calculatedNutrients.Fats.Should().Be(expectedNutrients.Fats);
            calculatedNutrients.Proteins.Should().Be(expectedNutrients.Proteins);
            calculatedNutrients.Carbohydrates.Should().Be(expectedNutrients.Carbohydrates);
        }

        public void EnsurePatchedUserHasExpectedResult(int userId, UserParams expectedUserParams)
        {
            var user = Context.Users.AsNoTracking().SingleOrDefault(u => u.Id == userId);

            user.Activity.Should().Be(expectedUserParams.ActivityEnum);
            user.Age.Should().Be(expectedUserParams.Age);
            user.Height.Should().Be(expectedUserParams.Height);
            user.Weight.Should().Be(expectedUserParams.Weight);
            user.Goal.Should().Be(expectedUserParams.GoalEnum);
            user.Gender.Should().Be(expectedUserParams.GenderEnum);
            user.Pregnancy.Should().Be(expectedUserParams.PregnancyEnum);
        }
    }
}
