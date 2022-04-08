namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public interface IPosDisplayManager
    {
        void StartWaitingForSwitchingToDisconnectedPage(int posId);
        void StopWaitingForSwitchingToDisconnectedPage(int posId);
    }
}
