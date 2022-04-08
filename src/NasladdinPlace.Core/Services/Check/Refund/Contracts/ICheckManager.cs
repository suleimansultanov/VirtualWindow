using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Check.Refund.Contracts
{
    public interface ICheckManager
    {
        event EventHandler<IEnumerable<CheckItem>> CheckItemsDeletedOrRefunded;
        event EventHandler<CheckEditingInfo> CheckItemsEditingCompleted;
        Task<ICheckManagerResult> RefundOrDeleteItemsAsync(CheckItemsEditingInfo checkItemsDeletionInfo);
        Task<ICheckManagerResult> AddItemAsync(CheckItemAdditionInfo checkItemAdditionInfo);
        Task<ICheckManagerResult> MarkCheckItemsAsVerifiedAsync(CheckItemsEditingInfo checkItemsEditingInfo);
    }
}