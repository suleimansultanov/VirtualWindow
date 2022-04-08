using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators
{
    public static class LastActivePosOperationDataGenerator
    {
        public const int ActiveOperationPosId = 1;
        public const int OtherPosId = 2;

        private static IEnumerable Data => new List<Collection<PosOperation>>
        {
            new Collection<PosOperation>
            {
                CreatePosOperation(ActiveOperationPosId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(ActiveOperationPosId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-2)),
                CreateCompletedPosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(ActiveOperationPosId, DateTime.UtcNow),
                CreatePosOperation(OtherPosId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-3)),
                CreateCompletedPosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-2)),
                CreatePosOperation(ActiveOperationPosId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(OtherPosId, DateTime.UtcNow)
            }
        };
        
        private static PosOperation CreateCompletedPosOperation(int posId, DateTime dateStarted)
        {
            var posOperation = CreatePosOperationOfUserAndPosBuilder(posId, dateStarted)
                .MarkAsCompleted()
                .Build();
            return posOperation;
        }

        private static PosOperation CreatePosOperation(int posId, DateTime dateStarted)
        {
            var shopOperation = CreatePosOperationOfUserAndPosBuilder(posId, dateStarted).Build();
            return shopOperation;
        }

        private static PosOperationOfUserAndPosBuilder CreatePosOperationOfUserAndPosBuilder(int posId, DateTime dateStarted)
        {
            return PosOperation.NewOfUserAndPosBuilder(1, posId).SetDateStarted(dateStarted);
        }
    }
}