using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Core.Services.Purchase.Initiation.Models
{
    public class PurchaseInitiationParams
    {
        public int UserId { get; }
        public string QrCode { get; }
        public Brand Brand { get; set; }
        public PosDoorPosition? DoorPosition { get; }

        public PurchaseInitiationParams(int userId, string qrCode)
        {
            if (userId < 0)
                throw new ArgumentException(nameof(userId), $"User id must be greater than zero, but its value is {userId}.");
            if (string.IsNullOrWhiteSpace(qrCode))
                throw new ArgumentNullException(nameof(qrCode));

            UserId = userId;
            QrCode = qrCode;
        }

        public PurchaseInitiationParams(int userId, 
            string qrCode, 
            PosDoorPosition? doorPosition) 
            : this(userId, qrCode)
        {
            DoorPosition = doorPosition;
        }
    }
}