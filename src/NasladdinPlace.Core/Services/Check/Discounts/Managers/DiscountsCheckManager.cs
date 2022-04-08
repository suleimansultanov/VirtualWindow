using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Services.Check.Discounts.Models;
using NasladdinPlace.Core.Services.Check.Discounts.Rules;
using NasladdinPlace.Core.Services.Check.Factory;

namespace NasladdinPlace.Core.Services.Check.Discounts.Managers
{
    public class DiscountsCheckManager : IDiscountsCheckManager
    {
        private readonly Dictionary<DiscountRuleType, IRuleChecker> _rulesCheckers;

        public DiscountsCheckManager()
        {
            _rulesCheckers = new Dictionary<DiscountRuleType, IRuleChecker>
            {
                { DiscountRuleType.ExpirationDate, RuleCheckerFactory.Create(DiscountRuleType.ExpirationDate) },
                { DiscountRuleType.PurchaseStartDate, RuleCheckerFactory.Create(DiscountRuleType.PurchaseStartDate) }
            };
        }

        public async Task AddDiscountsAsync(PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            if (!posOperation.CheckItems.Any())
                return;

            await AddDiscountsToCheckItems(posOperation, unitOfWork);
        }

        private async Task AddDiscountsToCheckItems(PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            var checkDiscounts = await GetPosOperationDiscountsAsync(posOperation, unitOfWork);

            foreach (var checkItem in posOperation.CheckItems)
            {
                var checkItemDiscount = checkDiscounts
                    .Where(c => c.CheckItemId == checkItem.Id)
                    .OrderByDescending(c => c.DiscountInPercentage)
                    .FirstOrDefault();

                if (checkItemDiscount == null)
                    continue;

                checkItem.AddDiscount(checkItemDiscount.DiscountInPercentage);
            }
        }

        private Task<List<CheckItemDiscount>> GetPosOperationDiscountsAsync(PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return GetPosOperationDiscountsAuxAsync(posOperation, unitOfWork);
        }

        private async Task<List<CheckItemDiscount>> GetPosOperationDiscountsAuxAsync(PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            var checkDiscounts = new List<CheckItemDiscount>();

            var posDiscounts = unitOfWork.Discounts.GetActiveDiscountsIncludeRulesByPosId(posOperation.PosId).ToList();

            if (!posDiscounts.Any())
                return new List<CheckItemDiscount>();

            foreach (var discount in posDiscounts)
            {
                var isMatchedAllRulesConditions = true;
                foreach (var rule in discount.DiscountRules)
                {
                    if (!_rulesCheckers.ContainsKey(rule.RuleType))
                    {
                        isMatchedAllRulesConditions = false;
                        break;
                    }

                    if (!await _rulesCheckers[rule.RuleType].IsMatchedAsync(rule, posOperation, unitOfWork))
                        isMatchedAllRulesConditions = false;
                }

                if (!isMatchedAllRulesConditions)
                    continue;

                var checkItemsDiscounts = await GetCheckItemsDiscountsAsync(discount, posOperation, unitOfWork);
                checkDiscounts.AddRange(checkItemsDiscounts);
            }

            return checkDiscounts;
        }

        private async Task<List<CheckItemDiscount>> GetCheckItemsDiscountsAsync(Discount discount,
            PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            switch (discount.DiscountArea)
            {
                case DiscountArea.Check:
                    return posOperation.CheckItems
                        .Select(c => new CheckItemDiscount(c.Id, discount.DiscountInPercentage))
                        .ToList();
                case DiscountArea.Good:
                    return await GetCheckItemsWithExpirationDateAsync(discount, posOperation, unitOfWork);
                default:
                    return new List<CheckItemDiscount>();
            }
        }

        private async Task<List<CheckItemDiscount>> GetCheckItemsWithExpirationDateAsync(Discount discount,
            PosOperation posOperation, IUnitOfWork unitOfWork)
        {
            var rule = discount.DiscountRules.FirstOrDefault(d => d.RuleType == DiscountRuleType.ExpirationDate);
            if (rule == null)
                return new List<CheckItemDiscount>();

            var ruleValue =
                rule.DiscountRuleValues.FirstOrDefault(d => d.ValueType == DiscountRuleValueType.SingleValue);
            if (ruleValue == null)
                return new List<CheckItemDiscount>();

            if (!int.TryParse(ruleValue.Value, out var ruleExpirationHours))
                return new List<CheckItemDiscount>();

            var expirationDate = DateTime.UtcNow.AddHours(ruleExpirationHours);
            var labelIds = posOperation.CheckItems.Select(c => c.LabeledGoodId).ToList();

            var labeledGoods = await unitOfWork.LabeledGoods.GetByIdsAndExpirationDateAsync(labelIds, expirationDate);

            if (!labeledGoods.Any())
                return new List<CheckItemDiscount>();

            var labeledGoodsIds = labeledGoods.Select(l => l.Id);
            return posOperation.CheckItems
                .Where(c => labeledGoodsIds.Contains(c.LabeledGoodId))
                .Select(c => new CheckItemDiscount(c.Id, discount.DiscountInPercentage))
                .ToList();
        }
    }
}
