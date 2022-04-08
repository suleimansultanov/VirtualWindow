using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using System;

namespace NasladdinPlace.DAL.Entities
{
    public class FeedbackEntity : Entity
    {
        // EF includes
        public ApplicationUser User { get; private set; }
        public Pos Pos { get; private set; }

        public int? UserId { get; private set; }
        public int? PosId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string PhoneNumber { get; private set; }
        public string DeviceName { get; private set; }
        public string DeviceOperatingSystem { get; private set; }
        public string AppVersion { get; private set; }
        public string Content { get; private set; }

        protected FeedbackEntity()
        {
            // required for EF
        }

        public FeedbackEntity(
            SenderInfo senderInfo,
            DateTime dateCreated,
            string appVersion,
            string content)
        {
            UserId = senderInfo.User?.Id;
            PosId = senderInfo.LastPurchaseInfo?.PosOperation?.PosId;
            DateCreated = dateCreated;
            PhoneNumber = senderInfo.PhoneNumber;
            DeviceName = senderInfo.DeviceInfo.DeviceName;
            DeviceOperatingSystem = senderInfo.DeviceInfo.OperatingSystem;
            AppVersion = appVersion;
            Content = content;
        }
    }
}