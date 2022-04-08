namespace NasladdinPlace.Api.Services.Pos.Display.Agents.Models
{
    public class PosDisplaySettings
    {
        public int WaitingSwitchingToDisconnectPageInSeconds { get; private set; }

        public int RetryShowQrCodeAfterInSeconds { get; private set; }

        public int RetryHideQrCodeAfterInSeconds { get; private set; }

        public int RetryShowTimerAfterInSeconds { get; private set; }

        public int CheckCommandsForRetrySendInSeconds { get; private set; }

        public PosDisplaySettings(int waitingSwitchingToDisconnectPageInSeconds,
                                  int retryShowQrCodeAfterInSeconds,
                                  int retryHideQrCodeAfterInSeconds,
                                  int retryShowTimerAfterInSeconds,
                                  int checkCommandsForRetrySendInSeconds)
        {
            CheckCommandsForRetrySendInSeconds = checkCommandsForRetrySendInSeconds;
            WaitingSwitchingToDisconnectPageInSeconds = waitingSwitchingToDisconnectPageInSeconds;
            RetryShowQrCodeAfterInSeconds = retryShowQrCodeAfterInSeconds;
            RetryHideQrCodeAfterInSeconds = retryHideQrCodeAfterInSeconds;
            RetryShowTimerAfterInSeconds = retryShowTimerAfterInSeconds;
        }
    }
}
