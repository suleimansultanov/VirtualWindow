using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class UserBonusPoint : Entity
    {
        public ApplicationUser User { get; private set; }
        
        public int UserId { get; private set; }
        public decimal Value { get; private set; }
        public BonusType Type { get; private set; }
        public DateTime DateCreated { get; private set; }
        public PosOperationTransaction PosOperationTransaction { get; private set; }
        public int? PosOperationTransactionId { get; private set; }
        public decimal AvailableBonusPoints { get; private set; }

        protected UserBonusPoint()
        {
            DateCreated = DateTime.UtcNow;
        }

        public UserBonusPoint(int userId, decimal value, BonusType type) : this()
        {
            UserId = userId;
            Value = value;
            Type = type;
        }
    }
}