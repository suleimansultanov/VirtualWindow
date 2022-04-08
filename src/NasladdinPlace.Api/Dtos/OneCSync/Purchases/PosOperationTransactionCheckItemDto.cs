using System;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationTransactionCheckItemDto
    {
        public int CheckItemId { get; set; }
        public int PosOperationTransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal CostInBonusPoints { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
