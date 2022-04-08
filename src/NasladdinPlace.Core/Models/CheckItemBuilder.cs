using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class CheckItemBuilder
    {
        private readonly CheckItem _checkItem;

        public CheckItemBuilder(int posId, int posOperationId, int goodId, int labeledGoodId, int currencyId)
        {
            _checkItem = new CheckItem(posId, posOperationId, goodId, labeledGoodId, currencyId);
        }

        public CheckItemBuilder SetPrice(decimal price)
        {
            _checkItem.Price = price;

            return this;
        }

        public CheckItemBuilder MarkAsModifiedByAdmin()
        {
            _checkItem.MarkAsModifiedByAdmin();

            return this;
        }

        public CheckItemBuilder SetStatus(CheckItemStatus status)
        {
            _checkItem.Status = status;

            return this;
        }

        public CheckItemBuilder SetCurrency(Currency currency)
        {
            _checkItem.Currency = currency;

            return this;
        }
        public CheckItemBuilder SetLabeledGood(LabeledGood labeledGood)
        {
            _checkItem.LabeledGood = labeledGood;

            return this;
        }

        public CheckItem Build()
        {
            return _checkItem;
        }
    }
}
