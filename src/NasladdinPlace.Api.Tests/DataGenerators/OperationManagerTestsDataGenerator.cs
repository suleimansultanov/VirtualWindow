using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Tests.DataGenerators.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Tests.DataGenerators
{
    public class OperationManagerTestsDataGenerator
    {
        private const int DefaultUserId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultPosOperationId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultCurrencyId = 1;

        private static IEnumerable Data => new List<OperationManagerSource>
        {
            new OperationManagerSource
            {
                PosOperations = new Collection<PosOperation>
                {
                    CreatePendingPaymentPosOperation(DefaultUserId, DateTime.UtcNow.AddHours(-1), new List<CheckItem>
                    {
                        CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Unpaid, 5M)
                    })
                },
                ExpectedStatus = PosOperationStatus.Paid,
                ExpectedBonus = 50M,
                ExpectedBonusesCount = 1
            },
            new OperationManagerSource
            {
                PosOperations = new Collection<PosOperation>
                {
                    CreatePendingPaymentPosOperation(DefaultUserId, DateTime.UtcNow, new List<CheckItem>
                    {
                        CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Unpaid, 5M)
                    }),
                    CreatePosOperation(DefaultUserId, DateTime.UtcNow.AddHours(-1), null)
                },
                ExpectedStatus = PosOperationStatus.Paid,
                ExpectedBonus = 50M,
                ExpectedBonusesCount = 1
            },
            new OperationManagerSource
            {
                PosOperations = new Collection<PosOperation>
                {
                    CreatePendingPaymentPosOperation(DefaultUserId, DateTime.UtcNow, new List<CheckItem>
                    {
                        CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Unpaid, 5M)
                    }),
                    CreatePosOperation(DefaultUserId, DateTime.UtcNow.AddHours(-1), new List<CheckItem>
                    {
                        CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Unpaid, 5M)
                    })
                },
                ExpectedStatus = PosOperationStatus.Paid,
                ExpectedBonus = 50M,
                ExpectedBonusesCount = 1
            }
        };

        private static PosOperation CreatePendingPaymentPosOperation(int userId, DateTime dateStarted, ICollection<CheckItem> checkItems)
        {
            var posOperation = CreatePosOperationOfUserAndPosBuilder(userId, dateStarted, checkItems)
                .MarkAsPendingPayment()
                .Build();

            return posOperation;
        }

        private static PosOperation CreateCompletedPosOperation(int userId, DateTime dateStarted, ICollection<CheckItem> checkItems)
        {
            var posOperation = CreatePosOperationOfUserAndPosBuilder(userId, dateStarted, checkItems)
                .MarkAsCompleted()
                .Build();

            return posOperation;
        }

        private static PosOperation CreatePosOperation(int userId, DateTime dateStarted, ICollection<CheckItem> checkItems)
        {
            var posOperation = CreatePosOperationOfUserAndPosBuilder(userId, dateStarted, checkItems).Build();

            return posOperation;
        }

        private static PosOperationOfUserAndPosBuilder CreatePosOperationOfUserAndPosBuilder(
            int userId, DateTime dateStarted, ICollection<CheckItem> checkItems)
        {
            var posOperationBuilder = PosOperation.NewOfUserAndPosBuilder(userId, 1)
                .SetDateStarted(dateStarted)
                .SetCheckItems(checkItems ?? new Collection<CheckItem>());

            return posOperationBuilder;
        }

        private static CheckItem CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus status, decimal price)
        {
            return CheckItem.NewBuilder(DefaultPosId, DefaultPosOperationId, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                .SetPrice(price)
                .SetStatus(status)
                .Build();
        }
    }
}