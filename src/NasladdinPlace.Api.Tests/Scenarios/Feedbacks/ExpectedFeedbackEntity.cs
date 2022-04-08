using System;

namespace NasladdinPlace.Api.Tests.Scenarios.Feedbacks
{
    public class ExpectedFeedbackEntity
    {
        public string AppVersion { get; }
        public string Content { get; }
        public string DeviceName { get; }
        public string DeviceOperatingSystem { get; }
        public string PhoneNumber { get; }
        public int? PosId { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; }

        public ExpectedFeedbackEntity(string appVersion, string content, string deviceName, string deviceOperatingSystem, string phoneNumber, DateTime dateCreated)
        {
            AppVersion = appVersion;
            Content = content;
            DeviceName = deviceName;
            DeviceOperatingSystem = deviceOperatingSystem;
            PhoneNumber = phoneNumber;
            DateCreated = dateCreated;
        }
    }
}