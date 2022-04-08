using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Discounts.Rules
{
    public class ExpirationDateRuleChecker : IRuleChecker
    {
        public Task<bool> IsMatchedAsync(DiscountRule rule, PosOperation posOperation, IUnitOfWork unitOfWork = null)
        {
            if(unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return IsMatchedAuxAsync(rule, posOperation, unitOfWork);
        }

        private async Task<bool> IsMatchedAuxAsync(DiscountRule rule, PosOperation posOperation, IUnitOfWork unitOfWork = null)
        {
            var ruleValue = rule.DiscountRuleValues.FirstOrDefault(d => d.ValueType == DiscountRuleValueType.SingleValue);

            if (ruleValue == null)
                return false;

            if (!int.TryParse(ruleValue.Value, out var ruleExpirationHours))
                return false;

            var expirationDate = DateTime.UtcNow.AddHours(ruleExpirationHours);
            var labelIds = posOperation.CheckItems.Select(c => c.LabeledGoodId).ToList();

            var labeledGoods = await unitOfWork.LabeledGoods.GetByIdsAndExpirationDateAsync(labelIds, expirationDate);

            return labeledGoods.Any();
        }
    }
}
