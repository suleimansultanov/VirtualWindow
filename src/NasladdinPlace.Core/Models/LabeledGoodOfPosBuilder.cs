namespace NasladdinPlace.Core.Models
{
    public class LabeledGoodOfPosBuilder
    {
        private readonly LabeledGood _labeledGood;
        
        public LabeledGoodOfPosBuilder(int posId, string label)
        {
            _labeledGood = LabeledGood.OfPos(posId, label);
        }

        public LabeledGoodOfPosBuilder SetId(int id)
        {
            _labeledGood.SetId(id);
            
            return this;
        }

        public LabeledGoodOfPosBuilder TieToGood(int goodId, LabeledGoodPrice price, ExpirationPeriod expirationPeriod)
        {
            _labeledGood.TieToGood(goodId, price, expirationPeriod);

            return this;
        }

        public LabeledGoodOfPosBuilder MarkAsUsedInPosOperation(int posOperationId)
        {
            _labeledGood.MarkAsUsedInPosOperation(posOperationId);

            return this;
        }

        public LabeledGoodOfPosBuilder Disable()
        {
            _labeledGood.Disable();

            return this;
        }
        
        public LabeledGood Build()
        {
            return _labeledGood;
        }
    }
}