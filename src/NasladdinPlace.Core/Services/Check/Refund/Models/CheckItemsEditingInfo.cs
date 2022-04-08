using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public class CheckItemsEditingInfo: ICheckItemInfo
    {
        public int PosOperationId { get; }
        public int? EditorId { get; }
        public ICollection<int> CheckItemsIds { get; set; }

        public static CheckItemsEditingInfo ForSystem(int posOperationId, IEnumerable<int> checkItemsIds)
        {
           return new CheckItemsEditingInfo(posOperationId, null, checkItemsIds); 
        }

        public static CheckItemsEditingInfo ForAdmin(int posOperationId, int editorId, IEnumerable<int> checkItemsIds)
        {
            return new CheckItemsEditingInfo(posOperationId, editorId, checkItemsIds);
        }

        private CheckItemsEditingInfo(int posOperationId, int? editorId, IEnumerable<int> checkItemsIds)
        {
            if (checkItemsIds == null)
                throw new ArgumentNullException(nameof(checkItemsIds));
            
            PosOperationId = posOperationId;
            EditorId = editorId;
            CheckItemsIds = checkItemsIds.ToImmutableList();
        }
    }
}