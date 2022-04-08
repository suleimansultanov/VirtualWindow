using System;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Documents.Base;

namespace NasladdinPlace.Core.Models.Documents.GoodsMoving
{
    public class DocumentGoodsMoving : DocumentBase<DocumentGoodsMovingTableItem>
    {
        public PosOperation PosOperation { get; private set; }
        public int PosOperationId { get; private set; }
        public Pos PointOfSale{ get; private set; }
        public int PosId { get; private set; }
        
        public DocumentGoodsMovingState State { get; private set; }

        protected DocumentGoodsMoving()
        {
            // required for EF
        }

        public DocumentGoodsMoving(PosOperation posOperation)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            PosOperation = posOperation;
            State = DocumentGoodsMovingState.Correct;
        }

        public DocumentGoodsMovingTableItem GetItemWithUntiedLabeledGoods()
        {
            return TablePart.FirstOrDefault(dti => dti.GoodId == null);
        }

        public void SetState(DocumentGoodsMovingState state)
        {
            if (!Enum.IsDefined(typeof(DocumentGoodsMovingState), state))
                throw new ArgumentException(nameof(state));

            State = state;
        }
    }
}
