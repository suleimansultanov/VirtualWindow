using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Services.Check.Discounts.Rules
{
    public class PurchaseStartDateRuleChecker : IRuleChecker
    {
        public Task<bool> IsMatchedAsync(DiscountRule rule, PosOperation posOperation, IUnitOfWork unitOfWork = null)
        {
            var minRuleValue = rule.DiscountRuleValues.FirstOrDefault(d => d.ValueType == DiscountRuleValueType.MinValue);
            var maxRuleValue = rule.DiscountRuleValues.FirstOrDefault(d => d.ValueType == DiscountRuleValueType.MaxValue);

            if (minRuleValue == null || maxRuleValue == null)
                return Task.FromResult(false);

            if (!TimeSpan.TryParse(minRuleValue.Value, out var minValue) ||
                !TimeSpan.TryParse(maxRuleValue.Value, out var maxValue))
                return Task.FromResult(false);

            var moscowTimeOfPurchase = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(posOperation.DateStarted).TimeOfDay;

            if (minValue >= maxValue)
                return Task.FromResult(moscowTimeOfPurchase >= minValue || moscowTimeOfPurchase <= maxValue);

            return Task.FromResult(moscowTimeOfPurchase >= minValue && moscowTimeOfPurchase <= maxValue);
        }
    }
}
