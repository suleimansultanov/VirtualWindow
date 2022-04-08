using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Interfaces;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers
{
    public interface IPosOperationTransactionCheckItemsMaker
    {
        List<PosOperationTransactionCheckItem> MakeCheckItems(decimal bonusAmount, IReadOnlyList<IReadonlyCheckItem> checkItems);
    }
}
