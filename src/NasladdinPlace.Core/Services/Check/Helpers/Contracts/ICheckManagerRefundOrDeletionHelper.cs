using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers.Contracts
{
    public interface ICheckManagerRefundOrDeletionHelper
    {
        Task<CheckManagerResult> MakeRefundOrDeleteAsync(
            CheckItemsEditingInfo checkItemsDeletionInfo,
            Action<ICollection<CheckItem>> notifyCheckItemsDeletedOrRefunded);
    }
}
