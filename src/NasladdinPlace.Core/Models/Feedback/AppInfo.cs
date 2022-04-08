namespace NasladdinPlace.Core.Models.Feedback
{
    public class AppInfo
    {
        public string AppVersion { get; }

        public AppInfo(string appVersion)
        {
            AppVersion = appVersion;
        }
    }
}