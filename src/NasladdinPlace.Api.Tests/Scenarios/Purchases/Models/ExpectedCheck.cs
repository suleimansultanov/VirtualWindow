using System;
using System.Collections.Generic;
using System.Text;

namespace NasladdinPlace.Api.Tests.Scenarios.Purchases.Models
{
    public class ExpectedCheck
    {
        public int PosOperationId { get; }
        public int ExpectedQuantity { get; }
        public decimal ExpectedTotalPrice { get; }
        public decimal ExpectedTotalPriceWithDiscount { get; }
        public decimal ExpectedBonus { get; }
        public int ExpectedCheckItemsCount { get; }
        public DateTime ExpectedDateTime { get; }

        public ExpectedCheck(
            int id,
            int quantity,
            decimal totalPrice, 
            int expectedCheckItemsCount, 
            DateTime expectedDateTime,
            decimal expectedTotalPriceWithDiscount,
            decimal expectedBonus)
        {
            PosOperationId = id;
            ExpectedQuantity = quantity;
            ExpectedTotalPrice = totalPrice;
            ExpectedCheckItemsCount = expectedCheckItemsCount;
            ExpectedDateTime = expectedDateTime;
            ExpectedTotalPriceWithDiscount = expectedTotalPriceWithDiscount;
            ExpectedBonus = expectedBonus;
        }
    }
}
