using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckSummary
    {
        public static DetailedCheckSummary FromGoods(ICollection<DetailedCheckGood> checkGoods, decimal bonus)
        {
            var initialTotalPrice = 0.0M;
            var initialTotalQuantity = 0;
            var actualTotalPrice = 0.0M;
            var actualTotalQuantity = 0;
            var hasUnverifiedItems = false;
            var initialTotalDiscount = 0.0M;
            var actualTotalDiscount = 0.0M;

            foreach (var checkGood in checkGoods)
            {
                initialTotalPrice += checkGood.Summary.InitialTotalPrice;
                initialTotalQuantity += checkGood.Summary.InitialTotalQuantity;
                actualTotalPrice += checkGood.Summary.ActualTotalPrice;
                actualTotalQuantity += checkGood.Summary.ActualTotalQuantity;
                initialTotalDiscount += checkGood.Summary.InitialTotalDiscount;
                actualTotalDiscount += checkGood.Summary.ActualTotalDiscount;

                if (checkGood.Summary.HasUnverifiedItems)
                {
                    hasUnverifiedItems = true;
                }
            }

            var initialCheckSummaryTotals =
                new CheckSummaryTotals(initialTotalPrice, initialTotalQuantity, initialTotalDiscount);

            var actualCheckSummaryTotals =
                new CheckSummaryTotals(actualTotalPrice, actualTotalQuantity, actualTotalDiscount);
            return new DetailedCheckSummary(
                initialCheckSummaryTotals,
                actualCheckSummaryTotals,
                bonus,
                hasUnverifiedItems
            );
        }

        public static DetailedCheckSummary FromGoodInstances(ICollection<DetailedCheckGoodInstance> checkGoodInstances)
        {
            var initialTotalPrice = checkGoodInstances.Sum(cgi => cgi.Price);
            var initialTotalQuantity = checkGoodInstances.Count;
            var hasUnverifiedItems = checkGoodInstances.Any(cgi => cgi.Status == CheckItemStatus.Unverified || cgi.Status == CheckItemStatus.PaidUnverified);
            var initialTotalDiscount = checkGoodInstances.Sum(cgi => cgi.Discount);

            var unpaidOrPaidCheckGoodInstances = checkGoodInstances
                .Where(cgi => cgi.Status == CheckItemStatus.Unpaid || cgi.Status == CheckItemStatus.Paid || cgi.Status == CheckItemStatus.PaidUnverified)
                .ToImmutableList();

            var actualTotalPrice = unpaidOrPaidCheckGoodInstances.Sum(item => item.Price);
            var actualTotalQuantity = unpaidOrPaidCheckGoodInstances.Count;
            var actualTotalDiscount = unpaidOrPaidCheckGoodInstances.Sum(item => item.Discount);

            var initialCheckSummaryTotals =
                new CheckSummaryTotals(initialTotalPrice, initialTotalQuantity, initialTotalDiscount);

            var actualCheckSummaryTotals =
                new CheckSummaryTotals(actualTotalPrice, actualTotalQuantity, actualTotalDiscount);

            return new DetailedCheckSummary(
                initialCheckSummaryTotals,
                actualCheckSummaryTotals,
                0.0M,
                hasUnverifiedItems
            );
        }

        public decimal InitialTotalPrice { get; }
        public int InitialTotalQuantity { get; }
        public int ActualTotalQuantity { get; }
        public decimal ActualTotalPrice { get; }
        public decimal Bonus { get; }
        public decimal InitialTotalDiscount { get; }
        public decimal ActualTotalDiscount { get; }
        public decimal ActualPriceWithDiscount { get; }
        public bool HasUnverifiedItems { get; }

        private DetailedCheckSummary(
            CheckSummaryTotals initialCheckSummaryTotals,
            CheckSummaryTotals actualCheckSummaryTotals,
            decimal bonus,
            bool hasUnverifiedItems)
        {
            InitialTotalPrice = initialCheckSummaryTotals.TotalPrice;
            InitialTotalQuantity = initialCheckSummaryTotals.TotalQuantity;
            InitialTotalDiscount = initialCheckSummaryTotals.TotalDiscount;
            ActualTotalPrice = actualCheckSummaryTotals.TotalPrice;
            ActualTotalQuantity = actualCheckSummaryTotals.TotalQuantity;
            ActualTotalDiscount = actualCheckSummaryTotals.TotalDiscount;
            Bonus = bonus;
            ActualPriceWithDiscount = Math.Max(ActualTotalPrice - ActualTotalDiscount, 0);
            HasUnverifiedItems = hasUnverifiedItems;
        }
    }
}