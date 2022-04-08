namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public class CheckItemAdditionInfo : ICheckItemInfo
    {
        public int PosOperationId { get; }
        public int GoodId { get; }
        public int LabeledGoodId { get; }
        public int? EditorId { get; }
        public int CurrencyId { get; }
        public decimal Price { get; }

        public static CheckItemAdditionInfo ForAdmin(int posOperationId, int goodId, int labeledGoodId, int currencyId,
            int editorId, decimal price)
        {
            return new CheckItemAdditionInfo(posOperationId, goodId, labeledGoodId, currencyId, editorId, price);
        }

        private CheckItemAdditionInfo(int posOperationId, int goodId, int labeledGoodId, int currencyId,
            int? editorId, decimal price)
        {
            PosOperationId = posOperationId;
            GoodId = goodId;
            LabeledGoodId = labeledGoodId;
            CurrencyId = currencyId;
            Price = price;
            EditorId = editorId;
        }
    }
}