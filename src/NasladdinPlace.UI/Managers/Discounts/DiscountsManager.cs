using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Logging;
using NasladdinPlace.UI.ViewModels.Discounts;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.Discounts
{
    public class DiscountsManager : IDiscountsManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;

        public DiscountsManager(IUnitOfWorkFactory unitOfWorkFactory, ILogger logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public bool Validate(DiscountInfoViewModel discountInfo, out string message)
        {
            message = string.Empty;

            var validationResults = new List<string>();
            if (!Enum.IsDefined(typeof(DiscountArea), discountInfo.DiscountArea))
                validationResults.Add("Невозможно определить тип области действия скидки");

            if (discountInfo.DiscountInPercents > 100)
                validationResults.Add("Скидка не может быть больше 100%");

            if (discountInfo.DiscountInPercents < 0)
                validationResults.Add("Скидка не может быть меньше 0%");

            if (string.IsNullOrEmpty(discountInfo.Name))
                validationResults.Add("Необходимо задать название скидки");

            if (discountInfo.DiscountId > 0)
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var discount = unitOfWork.GetRepository<Discount>().GetById(discountInfo.DiscountId);
                    if (discount == null)
                        validationResults.Add("Информация о скидке не найдена");
                }
            }

            if (!validationResults.Any())
                return true;

            message = string.Join(". ", validationResults);
            return false;
        }

        public async Task AddAsync(DiscountInfoViewModel discountInfoViewModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var discount = new Discount(discountInfoViewModel.Name,
                                            discountInfoViewModel.DiscountInPercents,
                                            (DiscountArea)discountInfoViewModel.DiscountArea,
                                            discountInfoViewModel.IsEnabled);

                unitOfWork.GetRepository<Discount>().Add(discount);

                AddPoses(discount, discountInfoViewModel, unitOfWork);
                AddRules(discount, discountInfoViewModel, unitOfWork);

                await unitOfWork.CompleteAsync();
            }
        }

        private void AddPoses(Discount discount, DiscountInfoViewModel discountInfoViewModel, IUnitOfWork unitOfWork)
        {
            if (!discountInfoViewModel.PointsOfSale.Any()) return;

            var posDiscounts = discountInfoViewModel.PointsOfSale
                                                    .Select(p => new PosDiscount(p.PosId, discount.Id))
                                                    .ToList();

            foreach (var posDiscount in posDiscounts)
                unitOfWork.GetRepository<PosDiscount>().Add(posDiscount);
        }

        private void AddRules(Discount discount, DiscountInfoViewModel discountInfo, IUnitOfWork unitOfWork)
        {
            if (!discountInfo.Rules.Any()) return;

            var discountRuleRepository = unitOfWork.GetRepository<DiscountRule>();
            var discountRuleValueRepository = unitOfWork.GetRepository<DiscountRuleValue>();

            foreach (var rule in discountInfo.Rules)
            {
                var discountRule = new DiscountRule(discount.Id,
                                                    (DiscountRuleArea)rule.DiscountRuleArea,
                                                    (DiscountRuleType)rule.DiscountRuleType);

                discountRuleRepository.Add(discountRule);

                var discountRuleValues = BuildDiscountRuleValues(rule, discountRule);
                foreach (var discountRuleValue in discountRuleValues)
                    discountRuleValueRepository.Add(discountRuleValue);
            }
        }

        public async Task UpdateAsync(DiscountInfoViewModel discountInfoViewModel, ApplicationUser user)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var discount = unitOfWork.Discounts.GetByIdIncludePosDiscountsAndRules(discountInfoViewModel.DiscountId);

                if (discount == null)
                    return;

                LogDiffirencies(discountInfoViewModel, user, discount);

                discount.Update(discountInfoViewModel.Name,
                                discountInfoViewModel.DiscountInPercents,
                                (DiscountArea)discountInfoViewModel.DiscountArea,
                                discountInfoViewModel.IsEnabled);

                var discountRuleRepository = unitOfWork.GetRepository<DiscountRule>();
                var discountRuleValueRepository = unitOfWork.GetRepository<DiscountRuleValue>();
                foreach (var discountRule in discount.DiscountRules)
                {
                    foreach (var discountRuleValue in discountRule.DiscountRuleValues)
                        discountRuleValueRepository.Remove(discountRuleValue.Id);

                    discountRuleRepository.Remove(discountRule.Id);
                }

                var posDiscountRepository = unitOfWork.GetRepository<PosDiscount>();
                foreach (var posDiscount in discount.PosDiscounts)
                    posDiscountRepository.Remove(posDiscount.Id);

                AddPoses(discount, discountInfoViewModel, unitOfWork);
                AddRules(discount, discountInfoViewModel, unitOfWork);

                await unitOfWork.CompleteAsync();
            }
        }

        private void LogDiffirencies(DiscountInfoViewModel discountInfoViewModel, ApplicationUser user, Discount discount)
        {
            try
            {
                var oldDiscountViewModel = GetDiscountViewModel(discount);

                if (oldDiscountViewModel.Equals(discountInfoViewModel))
                    return;

                var serializationSettings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                var logMessage =
                    $"The User={user.Id} has made editing discount with Id={discountInfoViewModel.DiscountId}. {Environment.NewLine}" +
                    $"OldValue = {JsonConvert.SerializeObject(oldDiscountViewModel, serializationSettings)}{Environment.NewLine}" +
                    $"NewValue = {JsonConvert.SerializeObject(discountInfoViewModel, serializationSettings)}";

                _logger.LogInfo(logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occurred while logging editing discounts. Error: {ex}");
            }
        }

        private List<DiscountRuleValue> BuildDiscountRuleValues(DiscountRuleViewModel rule, DiscountRule discountRule)
        {
            if (rule.DiscountRuleType == (int)DiscountRuleType.PurchaseStartDate)
                return new List<DiscountRuleValue>
                {
                    new DiscountRuleValue(discountRule.Id, DiscountRuleValueType.MinValue, rule.RuleValue.TimeFrom),
                    new DiscountRuleValue(discountRule.Id, DiscountRuleValueType.MaxValue, rule.RuleValue.TimeTo)
                };

            if (rule.DiscountRuleType == (int)DiscountRuleType.ExpirationDate)
                return new List<DiscountRuleValue>
                {
                    new DiscountRuleValue(discountRule.Id, DiscountRuleValueType.SingleValue, rule.RuleValue.HoursBeforeExpirationDate)
                };

            return new List<DiscountRuleValue>();
        }

        public DiscountInfoViewModel GetDiscountViewModel(Discount discount)
        {
            return new DiscountInfoViewModel
            {
                DiscountArea = (int)discount.DiscountArea,
                DiscountId = discount.Id,
                Name = discount.Name,
                DiscountInPercents = discount.DiscountInPercentage,
                IsEnabled = discount.IsEnabled,
                Rules = discount.DiscountRules.Select(r => new DiscountRuleViewModel
                {
                    DiscountRuleArea = (int)r.RuleArea,
                    DiscountRuleType = (int)r.RuleType,
                    RuleValue = BuildRuleValue(r)
                }).ToList(),
                PointsOfSale = discount.PosDiscounts.Select(p => new PosBasicInfoViewModel
                {
                    Name = p.Pos.Name,
                    PosId = p.PosId
                }).ToList()
            };
        }

        private static DiscountRuleValueViewModel BuildRuleValue(DiscountRule discountRule)
        {
            if (discountRule.RuleType == DiscountRuleType.PurchaseStartDate)
            {
                var minValue = discountRule.DiscountRuleValues.FirstOrDefault(r => r.ValueType == DiscountRuleValueType.MinValue);
                var maxValue = discountRule.DiscountRuleValues.FirstOrDefault(r => r.ValueType == DiscountRuleValueType.MaxValue);

                return new DiscountRuleValueViewModel
                {
                    TimeFrom = minValue != null ? TimeSpan.Parse(minValue.Value) : new TimeSpan(),
                    TimeTo = maxValue != null ? TimeSpan.Parse(maxValue.Value) : new TimeSpan()
                };
            }

            if (discountRule.RuleType == DiscountRuleType.ExpirationDate)
            {
                var ruleValue = discountRule.DiscountRuleValues.FirstOrDefault(r => r.ValueType == DiscountRuleValueType.SingleValue);

                return new DiscountRuleValueViewModel
                {
                    HoursBeforeExpirationDate = ruleValue != null ? int.Parse(ruleValue.Value) : 0
                };
            }

            return new DiscountRuleValueViewModel();
        }
    }
}
