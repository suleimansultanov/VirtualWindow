using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.PurchasesHistoryMaker.Models
{
    public class PurchaseHistory
    {
        public ICollection<SimpleCheck> Checks { get; }
        public SimpleCheck SimpleCheck { get; }

        public PurchaseHistory(IEnumerable<SimpleCheck> checks)
        {
            Checks = new ReadOnlyCollection<SimpleCheck>(checks.ToImmutableList());
        }

        public PurchaseHistory(SimpleCheck check)
        {
            SimpleCheck = check;
        }
    }
}