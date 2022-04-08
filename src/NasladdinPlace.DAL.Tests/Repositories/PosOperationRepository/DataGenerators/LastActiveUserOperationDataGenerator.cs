using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators
{
    public static class LastActiveUserOperationDataGenerator
    {
        public const int ActiveUserId = 1;
        public const int OtherUserId = 2;

        private static IEnumerable Data => new List<Collection<PosOperation>>
        {
            new Collection<PosOperation>
            {
                CreatePosOperation(ActiveUserId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(ActiveUserId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-2)),
                CreateCompletedPosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(ActiveUserId, DateTime.UtcNow),
                CreatePosOperation(OtherUserId, DateTime.UtcNow)
            },
            new Collection<PosOperation>
            {
                CreateCompletedPosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-3)),
                CreateCompletedPosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-2)),
                CreatePosOperation(ActiveUserId, DateTime.UtcNow.AddHours(-1)),
                CreatePosOperation(OtherUserId, DateTime.UtcNow)
            }
        };
        
        private static PosOperation CreateCompletedPosOperation(int userId, DateTime dateStarted)
        {
            var posOperation = CreatePosOperationOfUserAndPosBuilder(userId, dateStarted)
                .MarkAsCompleted()
                .Build();
            return posOperation;
        }

        private static PosOperation CreatePosOperation(int userId, DateTime dateStarted)
        {
            var shopOperation = CreatePosOperationOfUserAndPosBuilder(userId, dateStarted).Build();
            return shopOperation;
        }

        private static PosOperationOfUserAndPosBuilder CreatePosOperationOfUserAndPosBuilder(int userId, DateTime dateStarted)
        {
            return PosOperation.NewOfUserAndPosBuilder(userId, 1).SetDateStarted(dateStarted);
        }
    }
}