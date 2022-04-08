using System;

namespace NasladdinPlace.Core.Services.Purchase.Models
{
    public class PurchaseOperationParams
    {
        public int UserId { get; }
        
        public PurchaseOperationParams(int userId)
        {
            if (userId < 0)
                throw new ArgumentException(nameof(userId), $"User id must be greater than zero, but its value is {userId}.");

            UserId = userId;
        }
    }
}