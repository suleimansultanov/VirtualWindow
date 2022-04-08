namespace NasladdinPlace.Core.Models.Feedback
{
    public class DeviceInfo
    {
        public string DeviceName { get; }
        public string OperatingSystem { get; }

        public DeviceInfo(string deviceName, string operatingSystem)
        {
            DeviceName = deviceName;
            OperatingSystem = operatingSystem;
        }
    }
}