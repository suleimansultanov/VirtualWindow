using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators
{
    public class CheckItemsDataGenerator
    {
        public const int ActiveOperationPosId = 1;
        public const int ActiveFirstOperationId = 1;
        public const int ActiveSecondOperationId = 2;

        private static IEnumerable Data => new List<Collection<CheckItem>>
        {
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveFirstOperationId)
            },
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveSecondOperationId)
            },
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveFirstOperationId),
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 2, ActiveFirstOperationId)
            },
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveSecondOperationId),
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 2, ActiveSecondOperationId)
            },
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveSecondOperationId),
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Deleted, 2, ActiveSecondOperationId)
            },
            new Collection<CheckItem>
            {
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, 1, ActiveSecondOperationId),
                CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Refunded, 2, ActiveSecondOperationId)
            }
        };

        public static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId, int posOperationId)
        {
            return CheckItem.NewBuilder(posId, posOperationId, 1, labeledGoodId, 1)
                .SetPrice(5M)
                .SetStatus(status)
                .Build();
        }
    }
}
