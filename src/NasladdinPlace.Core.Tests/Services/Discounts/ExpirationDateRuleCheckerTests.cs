using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Check.Discounts.Rules;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.Discounts
{
    public class ExpirationDateRuleCheckerTests
    {
        private const int DefaultPosOperationId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultLabeledGoodId = 0;

        private const int DiscountRuleValueExpirationHours = 24;

        private readonly ExpirationDateRuleChecker _expirationDateRuleChecker;
        private readonly PosOperation _posOperation;
        private readonly LabeledGood _defaultLabeledGood;
        private readonly Mock<IUnitOfWork> _mockUoW;

        public ExpirationDateRuleCheckerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            var mockLabeledGoodRepository = new Mock<ILabeledGoodRepository>();

            LabeledGood.NewOfPosBuilder(DefaultPosId, "DefaultLabel")
                       .TieToGood(1, new LabeledGoodPrice(5, 1), new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddHours(12)))
                       .Build();

            _defaultLabeledGood = LabeledGood.NewOfPosBuilder(DefaultPosId, "DefaultLabel")
                                             .TieToGood(1, 
                                                        new LabeledGoodPrice(5, 1), 
                                                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), 
                                                                             DateTime.UtcNow.AddHours(12)))
                                             .Build();

            _posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetId(DefaultPosOperationId)
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        CheckItem.NewBuilder(
                                DefaultPosId, 
                                DefaultPosOperationId,
                                _defaultLabeledGood.GoodId.Value,
                                _defaultLabeledGood.Id, DefaultCurrencyId)
                            .SetPrice(10M)
                            .SetStatus(CheckItemStatus.Paid)
                            .Build()
                    })
                .Build();

            _mockUoW.SetupGet(u => u.LabeledGoods).Returns(mockLabeledGoodRepository.Object);
            
            mockLabeledGoodRepository
                .Setup(r => r.GetByIdsAndExpirationDateAsync(
                                It.Is<List<int>>(op => op.Contains(DefaultLabeledGoodId) && op.Count == 1),
                                It.Is<DateTime>(op => op > DateTime.UtcNow.AddHours(12))
                                )
                      ).Returns(
                        Task.FromResult(new List<LabeledGood> { _defaultLabeledGood }
                      ));

            _expirationDateRuleChecker = new ExpirationDateRuleChecker();
        }

        [Fact]
        public void IsMatched_ValidDiscountRule_ShouldReturnTrue()
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.SingleValue,
                    value: DiscountRuleValueExpirationHours
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.ExpirationDate
            );
            discountRule.DiscountRuleValues = discountRuleValues;

            var result = _expirationDateRuleChecker.IsMatchedAsync(discountRule, _posOperation, _mockUoW.Object).GetAwaiter().GetResult();

            result.Should().BeTrue();
        }

        [Fact]
        public void IsMatched_WrongDiscountRuleValue_ShouldReturnFalse()
        {
            var discountRuleValues = new List<DiscountRuleValue>
            {
                new DiscountRuleValue(
                    discountRuleId: 1,
                    valueType: DiscountRuleValueType.SingleValue,
                    value: "some_bad_value"
                )
            };

            var discountRule = new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.ExpirationDate
            );
            discountRule.DiscountRuleValues = discountRuleValues;

            var result = _expirationDateRuleChecker.IsMatchedAsync(discountRule, _posOperation, _mockUoW.Object).GetAwaiter().GetResult();

            result.Should().BeFalse();
        }
    }
}
