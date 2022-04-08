using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using System;

namespace NasladdinPlace.Core.Services.MicroNutrients.Calculator
{
    internal class BasicMetabolismCalculator
    {
        #region Constants

        private const double WeightCoefficient = 10;
        private const double HeightCoefficient = 6.25;
        private const double AgeCoefficient = 5;
        private const double CoefficientInMifflinSanGeorFormulaForMale = 5;
        private const double CoefficientInMifflinSanGeorFormulaForFemale = 161;

        private const int StandardBodyMassIndex = 25;

        #endregion

        private static double _userWeight;

        protected BasicMetabolismCalculator()
        {
            //intentionally left empty
        }

        public static double CalculateByMifflinSanGeorFormula(ApplicationUser user)
        {
            _userWeight = user.Weight.Value;

            if (user.Goal.Value == GoalType.DecreaseWeight)
                _userWeight = GetBodyMassIndexForFatMan(user);

            var basicMetabolismInCcal = CalculateBasicMetabolismByGender(user);

            return user.Goal == GoalType.IncreaseWeight
                ? RoundBasicMetabolismByGender(user.Gender, basicMetabolismInCcal)
                : basicMetabolismInCcal;
        }

        private static double RoundBasicMetabolismByGender(Gender gender, double basicMetabolismInCcal)
        {
            if (gender == Gender.Female)
            {
                var roundedFemaleBasicMetabolism = Math.Round(basicMetabolismInCcal + 0.2 * basicMetabolismInCcal, 0);
                return roundedFemaleBasicMetabolism;
            }

            var basicMaleMetabolism = basicMetabolismInCcal + 0.2 * basicMetabolismInCcal;
            return basicMaleMetabolism;
        }

        private static double CalculateBasicMetabolismByGender(ApplicationUser user)
        {
            if (user.Gender == Gender.Male)
            {
                var basicMaleMetabolism = Math.Round(
                    WeightCoefficient * _userWeight + HeightCoefficient * user.Height.Value -
                    AgeCoefficient * user.Age.Value + CoefficientInMifflinSanGeorFormulaForMale,
                    MidpointRounding.AwayFromZero);
                return basicMaleMetabolism;
            }

            var basicFemaleMetabolism = Math.Round(
                WeightCoefficient * _userWeight + HeightCoefficient * user.Height.Value -
                AgeCoefficient * user.Age.Value - CoefficientInMifflinSanGeorFormulaForFemale,
                MidpointRounding.AwayFromZero);
            return basicFemaleMetabolism;
        }

        private static double GetBodyMassIndexForFatMan(ApplicationUser user)
        {
            var bodyMassIndex = GetBodyMassIndex(user);
            return bodyMassIndex > StandardBodyMassIndex
                ? CalculateBodyMassIndexForFatManByFormula(bodyMassIndex, user.Height.Value)
                : _userWeight;
        }

        private static double CalculateBodyMassIndexForFatManByFormula(double bodyMassIndex, int height)
        {
            var bodyMassIndexForFatMan = bodyMassIndex * (1 - (bodyMassIndex - 25) / 100d) * Math.Pow(height / 100d, 2);
            return bodyMassIndexForFatMan;
        }

        private static double GetBodyMassIndex(ApplicationUser user)
        {
            var bodyMassIndex = Math.Round(_userWeight / Math.Pow(user.Height.Value / 100d, 2), 1);
            return bodyMassIndex;
        }
    }
}
