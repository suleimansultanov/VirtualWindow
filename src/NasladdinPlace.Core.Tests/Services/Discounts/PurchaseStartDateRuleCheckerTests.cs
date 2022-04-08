using System;
using System.Collections.Generic;
using FluentAssertions;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Services.Check.Discounts.Rules;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.Discounts
{
    public class PurchaseStartDateRuleCheckerTests
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        
        private readonly PurchaseStartDateRuleChecker _purchaseStartDateRuleChecker;
        private readonly PosOperation _posOperation;
        
        private readonly DateTime _nineOClockInThePast;

        public PurchaseStartDateRuleCheckerTests()
        {
            _purchaseStartDateRuleChecker = new PurchaseStartDateRuleChecker();

            _nineOClockInThePast = new DateTime(2019, 2, 1, 9, 0, 0, DateTimeKind.Utc);
            
            var startDateUtc = _nineOClockInThePast.Add(new TimeSpan(-7, 00, 00));

            _posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetDateStarted(startDateUtc)
                .Build();
        }

        [Fact]
        public void IsMatched_ValidDiscountRule_ShouldReturnTrue()
        {
            var discountRule = CreateDiscountRuleValues(minValue: 2, maxValue: 9);

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(24)]
        [InlineData(2)]
        [InlineData(3)]
        public void IsMatched_MinGreaterThanMaxDiscountRuleValuesAndPosOperationDateAfter12AM_ShouldReturnTrue(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 6,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: true);
        }

        [Theory]
        [InlineData(18)]
        [InlineData(20)]
        [InlineData(21)]
        public void IsMatched_MinGreaterThanMaxDiscountRuleValuesAndPosOperationDateBefore12AM_ShouldReturnTrue(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 6,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: true);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(7)]
        [InlineData(10)]
        [InlineData(14)]
        public void IsMatched_MinGreaterThanMaxDiscountRuleValuesAndPosOperationDateAfterMaxValue_ShouldReturnFalse(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 6,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: false);
        }

        [Theory]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        public void IsMatched_MinLessThanMaxDiscountRuleValuesAndPosOperationDateBefore12AM_ShouldReturnTrue(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 23,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: true);
        }

        [Theory]
        [InlineData(21)]
        [InlineData(24)]
        [InlineData(3)]
        [InlineData(14)]
        public void IsMatched_MinLessThanMaxDiscountRuleValuesAndPosOperationDateAfter12AM_ShouldReturnFalse(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 23,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: false);
        }

        [Fact]
        public void IsMatched_MinEqualsMaxDiscountRuleValues_ShouldReturnTrue()
        {
            var discountRule = CreateDiscountRuleValues(minValue: 5, maxValue: 5);

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(12)]
        [InlineData(14)]
        public void IsMatched_PosOperationDateStartedIsNotInDiscountPeriod_ShouldReturnFalse(double posOperationTimeInHours)
        {
            CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
                minValue: 18,
                maxValue: 6,
                posOperationTimeInHours: posOperationTimeInHours,
                expectedIsMatchedResult: false);
        }

        [Fact]
        public void IsMatched_DiscountRuleValuesNotSet_ShouldReturnFalse()
        {
           var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            );
           
            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeFalse();
        }

        [Fact]
        public void IsMatched_MinDiscountRuleValuesNotSet_ShouldReturnFalse()
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MaxValue,
                    value: _nineOClockInThePast.ToString("hh:mm:ss")
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            )
            {
                DiscountRuleValues = discountRuleValues
            };

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeFalse();
        }

        [Fact]
        public void IsMatched_MaxDiscountRuleValuesNotSet_ShouldReturnFalse()
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MinValue,
                    value: _nineOClockInThePast.Add(new TimeSpan(-7,00,00)).ToString("hh:mm:ss")
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            )
            {
                DiscountRuleValues = discountRuleValues
            };

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeFalse();
        }

        [Fact]
        public void IsMatched_WrongDiscountRuleValues_ShouldReturnFalse()
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MinValue,
                    value: "some_bad_value"
                ),
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MaxValue,
                    value: "some_bad_value"
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            )
            {
                DiscountRuleValues = discountRuleValues
            };

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, _posOperation).GetAwaiter().GetResult();

            result.Should().BeFalse();
        }

        private void CreateDiscountRuleValuesAndPosOperationAndAssertExpectedResult(
            int minValue,
            int maxValue,
            double posOperationTimeInHours,
            bool expectedIsMatchedResult)
        {
            var discountRule = CreateDiscountRuleValues(minValue, maxValue);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetDateStarted(_nineOClockInThePast.Date.AddHours(posOperationTimeInHours))
                .Build();

            var result = _purchaseStartDateRuleChecker.IsMatchedAsync(discountRule, posOperation).GetAwaiter().GetResult();

            if (expectedIsMatchedResult)
                result.Should().BeTrue();
            else
                result.Should().BeFalse();
        }

        private DiscountRule CreateDiscountRuleValues(int minValue, int maxValue)
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MinValue,
                    value: _nineOClockInThePast.Date.Add(TimeSpan.FromHours(minValue)).ToString("HH:mm:ss")
                ),
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.MaxValue,
                    value: _nineOClockInThePast.Date.Add(TimeSpan.FromHours(maxValue)).ToString("HH:mm:ss")
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            )
            {
                DiscountRuleValues = discountRuleValues
            };
            return discountRule;
        }
    }
}
