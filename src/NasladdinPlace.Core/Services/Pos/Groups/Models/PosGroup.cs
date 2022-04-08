using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.Core.Services.Pos.Groups.Models
{
    public class PosGroup<TElement>
    {   
        public PosShortInfo PosInfo { get; }

        private readonly ICollection<TElement> _items;

        public PosGroup(PosShortInfo posInfo)
        {
            PosInfo = posInfo;
            _items = new LinkedList<TElement>();
        }
        
        public int Id => PosInfo.Id;

        public IReadOnlyCollection<TElement> Items => _items.ToImmutableList();

        public void AddItem(TElement element)
        {
            _items.Add(element);
        }
    }
}